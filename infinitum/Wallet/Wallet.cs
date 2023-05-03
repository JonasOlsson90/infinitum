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
        _balance = GetCurrentBalance();
    }

    public string PrivateKey () => _privateKey;
    public string PublicKey () => _publicKey;
    public List<Block> Blockchain () => _blockchain;
    public decimal Balance () => _balance;

    private decimal GetCurrentBalance()
    {
        decimal balance = 0;

        // We need to assure that sending infinitum to your own wallet will not affect the balance
        foreach (var transaction in _blockchain.SelectMany(block => block.Transactions))
        {
            balance += transaction.Recipient == _publicKey ? transaction.Amount : 0;
            balance -= transaction.Sender == _publicKey ? transaction.Amount : 0;
        }

        return balance;
    }
}