using System.Security.Cryptography;
using infinitum.core;

namespace infinitum.IO;

public class IO
{
    private readonly SHA256 _sha256;

    public IO()
    {
        _sha256 = SHA256.Create();
    }

    public string GetPrivateKey()
    {
        // Check if local private key exists
        if (!File.Exists("./privatekey.txt"))
        {
            // If not generate a new one
            return Guid.NewGuid().ToString();
        }

        // ToDo: create from file
        return "";
    }

    public string GetPublicKey(string privateKey)
    {
        // Generate public key by hashing private key
        return _sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(privateKey)).ToString();
    }

    public List<Block> GetLocalBlockchain(string publicKey)
    {
        if (!File.Exists("./blockchain.txt"))
        {
            // If not create blockchain with genisis block

            // Create genisis block
            var transactions = new List<Transaction>();

            var transaction = new Transaction()
            {
                Amount = 1000,
                Sender = null,
                Recipient = publicKey,
                TimeStamp = DateTime.Now.Ticks
            };
            transactions.Add(transaction);

            var genisisBlock = new Block(0, _sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(String.Empty)), transactions);

            var blockchain = new List<Block>();

            blockchain.Add(genisisBlock);

            return blockchain;
        }

        //ToDo: Get from file
        return new List<Block>();
    }
}