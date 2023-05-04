using infinitum.DTOs;
using infinitum.Handlers;
using infinitum.Wallet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();


var io = new FileHandler();
var httpHandler = new HttpHandler();
var wallet = new Wallet(io, httpHandler);

// ToDo: Remove these and add real endpoints
app.MapGet("/ping", () => "I has been pinged");
app.MapGet("/testPK", () => wallet.PrivateKey);
app.MapGet("/testBC", () => wallet.Blockchain);
app.MapGet("/check_balance", () => wallet.Balance);
app.MapGet("/public_key", () => wallet.PublicKey);


app.MapPost($"/{wallet.PublicKey}", (IncomingTransactionDto incomingTransactionDto) => wallet.ReceiveTransaction(incomingTransactionDto));

app.MapPost("/send_transaction", (OutgoingTransactionDto outgoingTransactionDto ) => wallet.SendTransaction(outgoingTransactionDto));

app.Run();
