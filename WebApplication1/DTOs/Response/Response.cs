namespace Food_Ordering.DTOs.Response
{
    public class Response<T>
    {
        public string? Error { get; set; }
        public bool Status { get; set; }
        public T Data { get; set; }

        public Response(string message, bool status, T data)
        {
            Error = message;
            Status = status;
            Data = data;
        }

        public static Response<T> Success(T data)
        {
            return new Response<T> (null, true, data);
        }

        public static Response<T> Fail (string error)
        {
            return new Response<T> (error, false, default(T));
        }
    }
}
