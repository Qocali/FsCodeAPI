using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "statusCode")]
        public int StatusCode { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
