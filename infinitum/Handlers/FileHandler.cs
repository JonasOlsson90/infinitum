﻿using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using infinitum.core;
using infinitum.core.Utils;

namespace infinitum.Handlers;

public class FileHandler : IFileHandler
{
    private readonly SHA256 _sha256;
    private readonly string _filePathPrivateKey;
    private readonly string _filePathBlockchain;

    public FileHandler()
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

        return ParsingTools.ConvertByteArrayToHexString(publicKey);
    }

    public List<Block> GetLocalBlockchain(string publicKey)
    {
        List<Block> blockchain;
        
        if (!File.Exists(_filePathBlockchain))
        {
            // Create genesis block if no blockchain was found
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

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        var serializedBlockchain = JsonSerializer.Serialize(blockchain, options);
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

        return new Block(0, _sha256.ComputeHash(new byte[]{0}), transactions);
    }
}