using infinitum.core;
using infinitum.IO;

namespace infinitum.Wallet;

public class Wallet
{
    private readonly FileIOHandler _io;
    private string _privateKey;
    private string _publicKey;
    private List<Block> _blockchain;
    private decimal _balance;

    public Wallet(FileIOHandler io)
    {
        _io = io;
        _privateKey = _io.GetPrivateKey();
        _publicKey = _io.GetPublicKey(_privateKey);
        _blockchain = _io.GetLocalBlockchain(_publicKey);
        SetBalance();
    }

    public string PrivateKey () => _privateKey;
    public string PublicKey () => _publicKey;
    public List<Block> Blockchain () => _blockchain;
    public decimal Balance () => _balance;

    private void SetBalance()
    {
        // We need to assure that sending infinitum to your own wallet will not affect the balance
        _balance = 0;
        foreach (var transaction in _blockchain.SelectMany(block => block.Transactions))
        {
            _balance += transaction.Recipient == _publicKey ? transaction.Amount : 0;
            _balance -= transaction.Sender == _publicKey ? transaction.Amount : 0;
        }
    }

    public void ReceivePayment(string sender, decimal amount)
    {
        // Create transaction
        var transactions = new List<Transaction>();

        var transaction = new Transaction()
        {
            Amount = amount,
            Sender = sender,
            Recipient = _publicKey,
            TimeStamp = DateTime.Now.Ticks
        };

        transactions.Add(transaction);

        // Create block
        var newBlock = new Block(_blockchain.Last().Height + 1, _blockchain.Last().Hash, transactions);

        // Add block to the blockchain
        _blockchain.Add(newBlock);

        // Save the updated blockchain to file
        _io.SaveBlockchain(_blockchain);

        // Calculate new balance
        SetBalance();
    }
}