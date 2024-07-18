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
builder.WebHost.UseUrls("http://*:4444");
builder.Services.AddCors(options =>
	options.AddDefaultPolicy(
		policy =>
		{
			policy.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod();
		}));

builder.Configuration.AddJsonFile("appsettings.json");
var connectionStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddPersistence(connectionStr);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();
