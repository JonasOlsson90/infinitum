using System.Security.Cryptography;
using infinitum.core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

// ToDo: Remove this and add real endpoints
app.MapGet("/ping", () => "I has been pinged");

var client = new HttpClient();

// Check if local blockchain exists

List<Block> blockchain;
string privateKey;
string publicKey;

var sha = SHA256.Create();


// Check if local private key exists
if (!File.Exists("./privatekey.txt"))
{
    // If not generate a new one
    privateKey = Guid.NewGuid().ToString();
}
else
{
    // ToDo: create from file
    privateKey = "";
}


// Generate public key by hashing private key
publicKey = sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(privateKey)).ToString();

if (!File.Exists("./blockchain.txt"))
{
    // If not create blockchain with genisis block

    // Create genisis block
    var transactions = new List<Transaction>();

    var transaction = new Transaction()
    {
        Amount = 1000,
        Sender = null,
        Recipient = publicKey,
        TimeStamp = DateTime.Now.Ticks
    };

    transactions.Add(transaction);

    var genisisBlock = new Block(0, sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes(String.Empty)), transactions);

    blockchain = new();

    blockchain.Add(genisisBlock);
}
else
{
    // ToDo: create from file
}


app.Run();
