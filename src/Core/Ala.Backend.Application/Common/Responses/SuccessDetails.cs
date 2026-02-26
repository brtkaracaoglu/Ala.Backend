using System.Text.Json.Serialization;

namespace Ala.Backend.Application.Common.Responses
{
    public class SuccessDetails
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "https://euro-scada.com/success";

        [JsonPropertyName("title")]
        public string Title { get; set; } = "Success";

        [JsonPropertyName("status")]
        public int Status { get; set; } = 200;

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = "İşlem başarılı.";

        [JsonPropertyName("meta")]
        public Dictionary<string, object>? Meta { get; set; }
    }

    public class SuccessDetails<T> : SuccessDetails
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}

