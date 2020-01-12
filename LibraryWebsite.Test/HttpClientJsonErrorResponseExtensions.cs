using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components
{
    /// <summary>
    /// Extension methods for working with JSON APIs.
    /// </summary>
    public static class HttpClientJsonErrorResponseExtensions
    {
        public static Task<ErrorResponse> GetJsonErrorResponseAsync(this HttpClient httpClient, string requestUri)
            => httpClient.SendJsonErrorResponseAsync(HttpMethod.Get, requestUri, null);

        public static Task<ErrorResponse> PostJsonErrorResponseAsync(this HttpClient httpClient, string requestUri, object content)
            => httpClient.SendJsonErrorResponseAsync(HttpMethod.Post, requestUri, content);

        public static Task<ErrorResponse> PutJsonErrorResponseAsync(this HttpClient httpClient, string requestUri, object content)
            => httpClient.SendJsonErrorResponseAsync(HttpMethod.Put, requestUri, content);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
        public static async Task<ErrorResponse> SendJsonErrorResponseAsync(this HttpClient httpClient, HttpMethod method, string requestUri, object content)
        {
            var requestJson = JsonSerializer.Serialize(content, JsonSerializerOptionsProvider.Options);
            using var request = new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                throw new System.Exception("Expected error response, got success!");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new ErrorResponse(response.StatusCode, errorContent);
        }

        public class ErrorResponse
        {
            public ErrorResponse(HttpStatusCode statusCode, string errorContent)
            {
                StatusCode = statusCode;
                ErrorContent = errorContent;
            }

            public HttpStatusCode StatusCode { get; }

            public string ErrorContent { get; }
        }

        internal static class JsonSerializerOptionsProvider
        {
            public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
        }
    }
}