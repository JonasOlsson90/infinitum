using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using infinitum.core;

namespace infinitum.IO;

public class IO
{
    private readonly SHA256 _sha256;
    private readonly string _filePathPrivateKey;
    private readonly string _filePathBlockchain;

    public IO()
    {
        _sha256 = SHA256.Create();
        _filePathPrivateKey = "./privatekey.txt";
        _filePathBlockchain = "./blockchain.txt";
    }

    public string GetPrivateKey()
    {
        //ToDo: Find a better place to keep the unprotected private key

        // Create new private key if no file is found
        if (!File.Exists(_filePathPrivateKey))
            File.WriteAllText(_filePathPrivateKey, Guid.NewGuid().ToString());
        
        return File.ReadAllText(_filePathPrivateKey);
    }

    public string GetPublicKey(string privateKey)
    {
        // Generate public key by hashing private key
        var publicKey = _sha256.ComputeHash(Encoding.ASCII.GetBytes(privateKey));

        var sb = new StringBuilder();

        foreach (var b in publicKey)
            sb.Append(b.ToString("x2"));

        var str = sb.ToString();

        return str;
    }

    public List<Block> GetLocalBlockchain(string publicKey)
    {
        List<Block> blockchain;

        // Check if local blockchain exists
        if (!File.Exists(_filePathBlockchain))
        {
            // If not create blockchain with genisis block

            // Create genesis block

            var genesisBlock = GenerateGenesisBlock(publicKey);

            blockchain = new List<Block>
            {
                genesisBlock
            };

            SaveBlockchain(blockchain);

            return blockchain;
        }

        var serializedBlockchain = File.ReadAllText(_filePathBlockchain);

        return JsonSerializer.Deserialize<List<Block>>(serializedBlockchain);
    }

    public void SaveBlockchain(List<Block> blockchain)
    {
        var serializedBlockchain = JsonSerializer.Serialize(blockchain);
        File.WriteAllText(_filePathBlockchain, serializedBlockchain);
    }

    private Block GenerateGenesisBlock(string publicKey)
    {
        var transactions = new List<Transaction>();

        var transaction = new Transaction()
        {
            Amount = 1000,
            Sender = null,
            Recipient = publicKey,
            TimeStamp = DateTime.Now.Ticks
        };
        transactions.Add(transaction);

        return new Block(0, _sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(string.Empty)), transactions);
    }
}