using infinitum.IO;
using infinitum.Wallet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

// ToDo: Remove this and add real endpoints
app.MapGet("/ping", () => "I has been pinged");


var client = new HttpClient();
var io = new FileIOHandler();
var wallet = new Wallet(io);

app.MapGet("/testPK", () => wallet.PrivateKey());
app.MapGet("/testBC", () => wallet.Blockchain());
app.MapGet("/check_balance", () => wallet.Balance());
app.MapGet("/public_key", () => wallet.PublicKey());
app.MapGet($"/{wallet.PublicKey()}/", (string sender, decimal amount) => wallet.ReceivePayment(sender, amount));

app.Run();
