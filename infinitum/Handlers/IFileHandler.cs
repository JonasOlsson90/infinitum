using infinitum.core;

namespace infinitum.Handlers;

public interface IFileHandler
{
    string GetPrivateKey();
    string GetPublicKey(string privateKey);
    List<Block> GetLocalBlockchain(string publicKey);
    void SaveBlockchain(List<Block> blockchain);
}