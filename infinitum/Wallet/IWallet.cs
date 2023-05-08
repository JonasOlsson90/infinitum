using infinitum.core;
using infinitum.DTOs;

namespace infinitum.Wallet;

public interface IWallet
{
    string PrivateKey { get; }
    string PublicKey { get; }
    List<Block> Blockchain { get; }
    decimal Balance { get; }
    IResult ReceiveTransaction(IncomingTransactionDto incomingTransactionDto);

    Task<IResult> SendTransactionAsync(OutgoingTransactionDto outgoingTransactionDto);
}