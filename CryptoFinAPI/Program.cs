using CryptoFinAPI.ApiClients;
using CryptoFinAPI.App_Code;
using CryptoFinAPI.Repository;
using CryptoFinAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// configure DI for application services
builder.Services.AddScoped<IPriceService, PriceService>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();

var configuration = builder.Configuration;
builder.Services.AddScoped<PriceDbContext>();

var services = builder.Services;
services.AddHttpClient<IExternalApiClient, ServiceClientBitsFinex>(client =>
{
    client.BaseAddress = new Uri("https://api-pub.bitfinex.com/v2/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
services.AddHttpClient<IExternalApiClient, ServiceClientBitstamp>(client =>
{
    client.BaseAddress = new Uri("https://www.bitstamp.net/api/v2/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();


var scope = app.Services.CreateScope();
var db_context = scope.ServiceProvider.GetService<PriceDbContext>();
db_context.Database.EnsureCreated();

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();

//app.Run("http://localhost:3000");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Set the Swagger UI at the root URL
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
    options.RoutePrefix = string.Empty; 
});

app.Run();

