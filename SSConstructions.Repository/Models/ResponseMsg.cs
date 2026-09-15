using System;
using System.Collections.Generic;
using System.Text;

namespace SSConstructions.Repository.Models
{
    public class ResponseMsg
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public int Id { get; set; }
    }
}
