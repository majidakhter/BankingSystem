using BankingAppDDD.Common.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Polly.Wrap;

namespace BankingAppDDD.Common.Polly
{
    public class PollyWrap<T> where T : class
    {
        private readonly IOptionsMonitor<PolyConfigSettings> _policyConfigSettings;
        private readonly ILogger _logger;
        private int _timeoutSeconds, _retryCount, _sleepTimeAfterFail, _errorCountForCircuitbreak, _circuitBreakDuration;
        private AsyncCircuitBreakerPolicy circuitBreakerPolicy;
        public PollyWrap(IOptionsMonitor<PolyConfigSettings> policyConfigSettings, ILogger logger)
        {
            _policyConfigSettings = policyConfigSettings;
            _logger = logger;

            var settings = _policyConfigSettings?.CurrentValue;
            _timeoutSeconds = (settings != null && settings.TimeOut > 0) ? settings.TimeOut : 30;
            _retryCount = (settings != null && settings.RetryCount > 0) ? settings.RetryCount : 3;
            _sleepTimeAfterFail = (settings != null && settings.SleepTimeAfterFailure > 0) ? settings.SleepTimeAfterFailure : 1000;
            _errorCountForCircuitbreak = (settings != null && settings.ErrorCountForCircuitbreak > 0) ? settings.ErrorCountForCircuitbreak : 20;
            _circuitBreakDuration = (settings != null && settings.CircuitbreakWaitInMilliseconds > 0) ? settings.CircuitbreakWaitInMilliseconds : 20000;
        }

        public AsyncPolicyWrap GetPolicyConfig(T request,string handlerName) 
        {
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(_timeoutSeconds), TimeoutStrategy.Pessimistic);
            var retryPolicy = Policy.Handle<Exception>((ex) =>
            {
                _logger.LogError((ex.InnerException != null) ? ex.InnerException.Message : ex.Message + "\r\n" + handlerName + "\r\n Handle retry FAILURE - " + JsonConvert.SerializeObject(request), ex);
                return !(ex is Exception);
            })
           .Or<Exception>((ex) =>
           {
               _logger.LogError((ex.InnerException != null) ? ex.InnerException.Message : ex.Message + "\r\n" + handlerName + "\r\n Handle retry FAILURE - " + JsonConvert.SerializeObject(request), ex);
               return true;
           })
           .WaitAndRetryAsync(retryCount: _retryCount, sleepDurationProvider: x => TimeSpan.FromMilliseconds(_sleepTimeAfterFail));

            circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(exceptionsAllowedBeforeBreaking: _errorCountForCircuitbreak, durationOfBreak: TimeSpan.FromMilliseconds(_circuitBreakDuration),
              onBreak: (ex, breakDelay) =>
              {
                  _logger.LogError(ex, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message + "\r\n "+ handlerName + " Circuit broken");
              },
              onReset: () => { });

            var pollyWrap = Policy.WrapAsync(circuitBreakerPolicy, timeoutPolicy, retryPolicy);

            return pollyWrap;
        }
    }
}
