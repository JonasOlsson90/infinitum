using infinitum.core;

namespace infinitum.Handlers;

public interface IHttpHandler
{
    Task<bool> PostTransactionAsync(Transaction transaction, string address);
    Task<string?> GetPublicKeyAsync(string address);
}