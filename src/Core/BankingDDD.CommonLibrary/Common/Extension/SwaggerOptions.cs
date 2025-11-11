using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDDD.Common.Extension
{
    public class SwaggerOptions
    {
        public bool Enabled { get; set; }
        public bool ReDocEnabled { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }
        public required string Version { get; set; }
        public required string RoutePrefix { get; set; }
        public bool IncludeSecurity { get; set; }
       
    }
    
}
