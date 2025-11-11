using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDDD.Domains.Extensions
{
    public record NotNegativeOrZero
    {
        public NotNegativeOrZero(int value) => Value = value.NotBeNegativeOrZero();
        public int Value { get; }
    }
}
