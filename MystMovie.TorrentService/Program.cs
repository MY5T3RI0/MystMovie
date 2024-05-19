using MystMovie.DB;
using MystMovie.Picker;
using MystMovie.Services;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ITorrentSearcher, TorrentSearcher>();
builder.Services.AddLogging();

builder.Configuration.AddJsonFile("appsettings.json");
var connectionStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddPersistence(connectionStr);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
