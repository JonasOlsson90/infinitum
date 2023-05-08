using System.Globalization;
using infinitum.core;
using infinitum.core.Utils;
using infinitum.DTOs;
using infinitum.Handlers;

namespace infinitum.Wallet;

public class Wallet : IWallet
{
    private const decimal BlockRewardMultiplier = 0.1M;

    private readonly IFileHandler _io;
    private readonly IHttpHandler _httpHandler;

    public string PrivateKey { get; }
    public string PublicKey { get; }
    public List<Block> Blockchain { get; }
    public decimal Balance { get; private set; }

    public Wallet(IFileHandler io, IHttpHandler httpHandler)
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
        Balance = 0;
        foreach (var transaction in Blockchain.SelectMany(block => block.Transactions))
        {
            Balance += transaction.Recipient == PublicKey ? transaction.Amount : 0;
            Balance -= transaction.Sender == PublicKey ? transaction.Amount : 0;
        }
    }

    public IResult ReceiveTransaction(IncomingTransactionDto incomingTransactionDto)
    {
        if (!Validator.ValidateBlockchain(Blockchain))
            return TypedResults.BadRequest("Invalid local blockchain");

        if (!decimal.TryParse(incomingTransactionDto.Amount.Replace(",", "."), CultureInfo.InvariantCulture, out var amount))
            return TypedResults.BadRequest("Amount is in incorrect format");

        var transaction = new Transaction()
        {
            Amount = amount,
            Sender = incomingTransactionDto.Sender,
            Recipient = PublicKey,
            TimeStamp = DateTime.Now.Ticks
        };

        HandleTransaction(transaction);
        return TypedResults.Ok("Successfully received transaction");
    }

    public async Task<IResult> SendTransactionAsync(OutgoingTransactionDto outgoingTransactionDto)
    {
        const int lengthOfSha256 = 64;

        if (!Validator.ValidateBlockchain(Blockchain))
            return TypedResults.BadRequest("Invalid local blockchain");

        if (!decimal.TryParse(outgoingTransactionDto.Amount.Replace(",", "."), CultureInfo.InvariantCulture, out var amount))
            return TypedResults.BadRequest("Amount is in incorrect format");

        if (Balance - amount < 0)
            return TypedResults.BadRequest("Insufficient funds");

        if (amount <= 0)
            return TypedResults.BadRequest("Sneaky! No negative transactions.");

        var recipientPublicKey = await _httpHandler.GetPublicKeyAsync(outgoingTransactionDto.Address);

        if (string.IsNullOrEmpty(recipientPublicKey) || recipientPublicKey.Length != lengthOfSha256)
            return TypedResults.BadRequest("Peer not online");

        var transaction = new Transaction()
        {
            Amount = amount,
            Sender = PublicKey,
            Recipient = recipientPublicKey,
            TimeStamp = DateTime.Now.Ticks
        };

        if (!await _httpHandler.PostTransactionAsync(transaction, outgoingTransactionDto.Address))
            return TypedResults.BadRequest("Transaction failed");

        HandleTransaction(transaction);

        return TypedResults.Ok("Transaction successful");
    }

    private void HandleTransaction(Transaction transaction)
    {
        var transactions = new List<Transaction>() {transaction};
        transactions.Add(GenerateBlockRewardTransaction(transactions));
        var newBlock = new Block(Blockchain.Last().Height + 1, Blockchain.Last().Hash, transactions);
        Blockchain.Add(newBlock);
        _io.SaveBlockchain(Blockchain);
        SetBalance();
    }

    private Transaction GenerateBlockRewardTransaction(IEnumerable<Transaction> transactions)
    {
        var totalSum = transactions.Sum(t => t.Amount);

        return new Transaction()
        {
            Amount = totalSum * BlockRewardMultiplier,
            Sender = "**BLOCK REWARD**",
            Recipient = PublicKey,
            TimeStamp = DateTime.Now.Ticks
        };
    }
}