using Microsoft.AspNetCore.Http;
namespace ArtEva.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string BuildPublicUrl(this HttpRequest request, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{relativePath.TrimStart('/')}";
        }
    }
}
