using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace site.models
{
    public class Response
    {
        public string status { get; set; }
        public dynamic data { get; set; }
        public string message { get; set; }
    }
    public static class ResponseStatus
    {
        public const string Success = "Success";
        public const string Error = "Error";
    }
}