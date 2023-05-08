using infinitum.DTOs;
using infinitum.Handlers;
using infinitum.Wallet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IFileHandler, FileHandler>();
builder.Services.AddScoped<IHttpHandler, HttpHandler>();
builder.Services.AddScoped<IWallet, Wallet>();

//var wallet = new Wallet();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/get_blockchain", (IWallet wallet) => wallet.Blockchain);
app.MapGet("/check_balance", (IWallet wallet) => wallet.Balance);
app.MapGet("/public_key", (IWallet wallet) => wallet.PublicKey);

app.MapPost("/{publicKey}", (IWallet wallet, string publicKey, IncomingTransactionDto incomingTransactionDto) 
    => publicKey == wallet.PublicKey
    ? wallet.ReceiveTransaction(incomingTransactionDto) 
    : TypedResults.NotFound());

app.MapPost("/send_transaction", (IWallet wallet, OutgoingTransactionDto outgoingTransactionDto ) 
    => wallet.SendTransactionAsync(outgoingTransactionDto));

//app.UseHttpsRedirection();

app.Run();
