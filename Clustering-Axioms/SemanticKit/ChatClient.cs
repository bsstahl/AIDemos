using System.Text;
using System.Text.Json;

namespace SemanticKit;

public class ChatClient : IGetChatCompletions
{
    const string _endpointUrl = "http://localhost:1234/v1/chat/completions";
    const string _modelName = "openai/gpt-oss-20b";

    private readonly HttpClient _httpClient;

    public ChatClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string GetChatCompletion(string context)
    {
        var payload = new
        {
            model = _modelName,
            messages = new[]
            {
                new { role = "user", content = context }
            }
        };

        string json = JsonSerializer.Serialize(payload);
        using var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = _httpClient.Send(request);
        response.EnsureSuccessStatusCode();

        using var stream = response.Content.ReadAsStream();
        using var doc = JsonDocument.Parse(stream);

        var content =
            doc.RootElement
               .GetProperty("choices")[0]
               .GetProperty("message")
               .GetProperty("content")
               .GetString();

        return content ?? string.Empty;
    }
}
