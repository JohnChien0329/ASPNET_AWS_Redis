using Microsoft.Extensions.Caching.Distributed;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "test1.wjfgxv.ng.0001.use1.cache.amazonaws.com:6379";
});

builder.Services.AddSingleton<Random>();
var app = builder.Build();

app.MapGet("/Inventory/{itemId}", (string itemId, IDistributedCache distributedCache, Random random) =>
{
    string? key = distributedCache.GetString(itemId);
    if (key != null)
    {

        
        return Results.Ok($"Found item:{itemId}, in the cache with id[{key}] in stock.");
    }
    else
    {
        
        key = random.Next(100).ToString();
        distributedCache.SetString(itemId, key, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        });
        return Results.Ok($"Could not find {itemId} in the cache! Store item in cache with id[{key}]");
    }
});






// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
