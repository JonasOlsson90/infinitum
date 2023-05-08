using infinitum.core;
using System.Globalization;
using infinitum.DTOs;

namespace infinitum.Handlers;

public class HttpHandler : IHttpHandler
{
    private readonly HttpClient _httpClient;
    
    public HttpHandler()
    {
        _httpClient = new();
    }

    public async Task<bool> PostTransactionAsync(Transaction transaction, string address)
    {
        if (string.IsNullOrEmpty(transaction.Sender))
            return false;

        var values = new IncomingTransactionDto()
        {
            Sender = transaction.Sender,
            Amount = transaction.Amount.ToString(CultureInfo.InvariantCulture),
        };

        var requestUri = $"{address}/{transaction.Recipient}";

        var response = await _httpClient.PostAsJsonAsync(requestUri, values);
        
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetPublicKeyAsync(string address)
    {
        return await _httpClient.GetStringAsync($"{address}/public_key");
    }
}