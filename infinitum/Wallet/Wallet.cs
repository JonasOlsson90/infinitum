using infinitum.core;
using infinitum.DTOs;
using infinitum.Handlers;
using System.Reflection;

namespace infinitum.Wallet;

public class Wallet
{
    private readonly FileHandler _io;
    private readonly HttpHandler _httpHandler;

    public string PrivateKey { get; private set; }
    public string PublicKey { get; private set; }
    public List<Block> Blockchain { get; private set; }
    public decimal Balance { get; private set; }

    public Wallet(FileHandler io, HttpHandler httpHandler)
    {
        _io = io;
        _httpHandler = httpHandler;
        PrivateKey = _io.GetPrivateKey();
        PublicKey = _io.GetPublicKey(PrivateKey);
        Blockchain = _io.GetLocalBlockchain(PublicKey);
        SetBalance();
    }

    private void SetBalance()
    {
        // We need to assure that sending infinitum to your own wallet will not affect the balance
        Balance = 0;
        foreach (var transaction in Blockchain.SelectMany(block => block.Transactions))
        {
            Balance += transaction.Recipient == PublicKey ? transaction.Amount : 0;
            Balance -= transaction.Sender == PublicKey ? transaction.Amount : 0;
        }
    }

    public void ReceiveTransaction(string sender, decimal amount)
    {
        var transaction = new Transaction()
        {
            Amount = amount,
            Sender = sender,
            Recipient = PublicKey,
            TimeStamp = DateTime.Now.Ticks
        };;

        HandleTransaction(transaction);
    }

    public async Task SendTransaction(OutgoingTransactionDto outgoingTransactionDto)
    {
        var transaction = new Transaction()
        {
            Amount = outgoingTransactionDto.Amount,
            Sender = PublicKey,
            Recipient = outgoingTransactionDto.Recipient,
            TimeStamp = DateTime.Now.Ticks
        };

        //ToDo: Display that something went wrong
        if (!await _httpHandler.PostTransaction(transaction, outgoingTransactionDto.Address))
            return;

        HandleTransaction(transaction);
    }

    private void HandleTransaction(Transaction transaction)
    {
        var transactions = new List<Transaction>() {transaction};
        var newBlock = new Block(Blockchain.Last().Height + 1, Blockchain.Last().Hash, transactions);
        Blockchain.Add(newBlock);
        _io.SaveBlockchain(Blockchain);
        SetBalance();
    }
}