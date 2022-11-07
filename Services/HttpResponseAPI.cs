using System.Net;

namespace DMNRestaurant.Services
{
    public static class HttpResponseAPI<T>
    {
        public static ResponseAPI<T> SResponse(
                bool IsSuccess = true,
                string errMessage = "",
                HttpStatusCode StatusCode = HttpStatusCode.OK,
                T? Data = default)
        {
            var response = new ResponseAPI<T>();

            if (!IsSuccess)
                response.ErrorMessage = new List<string> { errMessage };

            response.StatusCode = StatusCode;
            response.IsSuccess = IsSuccess;
            response.Data = Data;

            return response;
        }
    }
}
