using Common.Responses.Wrappers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Admin.Extensions
{
    public static class ResponseWrapperExtensions
    {
        internal static async Task<IResponseWrapper<T>> WrapResponse<T>(this HttpResponseMessage responseMessage)
        {
            var responseAsString = await responseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ResponseWrapper<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }

        internal static async Task<IResponseWrapper> WrapResponse(this HttpResponseMessage responseMessage)
        {
            var responseAsString = await responseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ResponseWrapper>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }
    }
}
