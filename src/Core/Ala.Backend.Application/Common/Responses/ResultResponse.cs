
namespace Ala.Backend.Application.Common.Responses
{
    public static class ResultResponse
    {
        // Veri dönmeyen başarılı işlem (200 OK)
        public static SuccessDetails Success(string messageKey)
        {
            return new SuccessDetails
            {
                StatusCode = 200,
                Detail = messageKey
            };
        }

        // Veri dönen başarılı işlem (200 OK)
        public static SuccessDetails<T> Success<T>(T data, string messageKey)
        {
            return new SuccessDetails<T>
            {
                StatusCode = 200,
                Detail = messageKey,
                Data = data
            };
        }

        // Oluşturuldu (201 Created)
        public static SuccessDetails<T> Created<T>(T data, string messageKey)
        {
            return new SuccessDetails<T>
            {
                StatusCode = 201,
                Detail = messageKey,
                Data = data
            };
        }
    }
}