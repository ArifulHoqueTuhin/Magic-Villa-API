using System.Net;

namespace MagicVillaApi.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessage { get; set; }
        public ApiResponse()
        {
            ErrorMessage = new List<string>();
        }
        public object? Result { get; set; }
    }
}
