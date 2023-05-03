using infinitum.core;

namespace infinitum.Handlers;

public class HttpHandler
{
    private readonly HttpClient _httpClient;
    
    public HttpHandler()
    {
        _httpClient = new();
    }

    public async Task<bool> PostTransaction(Transaction transaction, string address)
    {
        var request = $"{address}/{transaction.Recipient}?sender={transaction.Sender}&amount={transaction.Amount}";

        var response = await _httpClient.GetAsync(request);
        
        return response.IsSuccessStatusCode;
    }
}