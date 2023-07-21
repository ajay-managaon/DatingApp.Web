using System.Text.Json;
using Web.DatingApp.API.Web.DatingApp.Helpers;

namespace Web.DatingApp.API.Web.DatingApp.Extensions
{
    public static class HttpExtension
    {
        public static void AddPaginationHeader(this HttpResponse httpResponse, PaginationHeader header)
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            httpResponse.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            httpResponse.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
