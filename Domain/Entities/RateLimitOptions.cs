using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RateLimitOptions
    {
        public TimeSpan TimeSpan { get; set; }
        public int Requests { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
