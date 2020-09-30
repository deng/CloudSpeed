namespace CloudSpeed.Web.Responses
{
    public abstract class ApiResponse
    {
        public static ApiResponse<string> Ok()
        {
            return new ApiResponse<string>(200, null, null);
        }

        public static ApiResponse<T> Ok<T>(T data)
        {
            return new ApiResponse<T>(200, data, null);
        }

        public static ApiResponse<T> Result<T>(ApiError error)
        {
            return new ApiResponse<T>(error.Code, default, error.Error);
        }

        public static ApiResponse<T> BadRequestResult<T>(string error)
        {
            return new ApiResponse<T>(400, default, error);
        }

        public static ApiResponse<string> BadRequestResult(string error)
        {
            return new ApiResponse<string>(400, null, error);
        }

        public static ApiResponse<T> NotFound<T>(string error)
        {
            return new ApiResponse<T>(404, default, error);
        }

        public static ApiResponse<string> NotFound(string error)
        {
            return new ApiResponse<string>(404, null, error);
        }
    }

    public class ApiResponse<T>
    {
        public int Code { get; set; }

        public T Data { get; set; }

        public string Error { get; set; }

        public ApiResponse(int code, T data, string error)
        {
            Code = code;
            Data = data;
            Error = error;
        }

        public bool Success
        {
            get { return Code == 200; }
        }
    }

    public class ApiError
    {
        public static ApiError Create(int code, string error)
        {
            return new ApiError() { Code = code, Error = error };
        }

        public int Code { get; set; }

        public string Error { get; set; }
    }
}
