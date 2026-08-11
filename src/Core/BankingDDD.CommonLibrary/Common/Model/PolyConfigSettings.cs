
namespace BankingAppDDD.Common.Model
{
    public class PolyConfigSettings
    {
        public int TimeOut { get; set; }
        public int RetryCount { get; set; }
        public int SleepTimeAfterFailure { get; set; }
        public int ErrorCountForCircuitbreak { get; set; }
        public int CircuitbreakWaitInMilliseconds { get; set; }

    }
}
