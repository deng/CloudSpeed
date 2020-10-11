namespace CloudSpeed.Web.Responses
{
    public class AdminApiResponse<T>
    {
        public bool Status { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }
    }

    public static class ApiResponseExtension
    {
        public static AdminApiResponse<T> ToAdminResponse<T>(this ApiResponse<T> response)
        {
            return new AdminApiResponse<T>()
            {
                Status = response.Success,
                Data = response.Data,
                Message = response.Error
            };
        }
    }
}