using System;
using System.Collections.Generic;
using System.Text;

namespace BaseApi.Models
{
    public class ServiceResponse<T> where T : class
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }

    }
}
