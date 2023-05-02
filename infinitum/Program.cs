using infinitum.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

// ToDo: Remove this and add real endpoints
app.MapGet("/ping", () => "I has been pinged");


var client = new HttpClient();
var io = new IO();

// Check if local blockchain exists

var privateKey = io.GetPrivateKey();
var publicKey = io.GetPublicKey(privateKey);
var blockchain = io.GetLocalBlockchain(publicKey);

app.Run();
