using infinitum.core;
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
app.MapGet("/testPK", () => wallet.PrivateKey());
app.MapGet("/testBC", () => wallet.Blockchain());
app.MapGet("/check_balance", () => wallet.Balance());
app.MapGet("/public_key", () => wallet.PublicKey());


app.MapGet($"/{wallet.PublicKey()}/", (string sender, decimal amount) => wallet.ReceiveTransaction(sender, amount));

app.MapPost("/send_transaction", (Transaction transaction, string address ) => wallet.SendTransaction(transaction, address));

app.Run();
