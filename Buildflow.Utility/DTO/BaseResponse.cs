using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class BaseResponse<T>
    {
        public bool? Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class BaseResponse
    {
        public bool? Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
