using System.Net;

namespace DMNRestaurant.Services
{
    public class ResponseAPI<T>
    {
        public bool IsSuccess { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public List<string>? ErrorMessage { get; set; }
        public T? Data { get; set; }
    }
}
