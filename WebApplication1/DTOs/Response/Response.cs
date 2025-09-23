namespace Food_Ordering.DTOs.Response
{
    public class Response<T>
    {
        public string? Error { get; set; }
        public bool Status { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        public Response(string message, bool status, T data, int statusCode)
        {
            Error = message;
            Status = status;
            Data = data;
            StatusCode = statusCode;
        }

        public static Response<T> Success(T data, int code)
        {
            return new Response<T> (null, true, data, code);
        }

        public static Response<T> Fail (string error, int code)
        {
            return new Response<T> (error, false, default(T), code);
        }
    }
}
