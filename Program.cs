global using WebApi.Models;
global using Microsoft.EntityFrameworkCore;
global using WebApi.Data;
global using AutoMapper;
global using WebApi.Services.CartService;
global using WebApi.Services;
global using WebApi.Dtos.Cart;
global using WebApi.Dtos.Item;
global using WebApi.Dtos.Variant;
global using WebApi.Dtos.User;
global using WebApi.Dtos.Kit;
global using WebApi.Dtos.Order;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using WebApi;
using EFCoreSecondLevelCacheInterceptor;
using WebApi.Services.OrderService;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using EasyCaching.InMemory;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Add services to the container
const string providerName1 = "InMemory1";
builder.Services.AddEFSecondLevelCache(options =>
        options.UseEasyCachingCoreProvider(providerName1, isHybridCache: false).DisableLogging(true).UseCacheKeyPrefix("EF_")
        .CacheQueriesContainingTableNames(
                        CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30), TableNameComparison.ContainsOnly,
                        "Kits", "Variants", "Items", "Users", "Orders"
                    ));
builder.Services.AddEasyCaching(options =>
            {
                // use memory cache with your own configuration
                options.UseInMemory(config =>
                {
                    config.DBConfig = new InMemoryCachingOptions
                    {
                        // scan time, default value is 60s
                        ExpirationScanFrequency = 60,
                        // total count of cache items, default value is 10000
                        SizeLimit = 100,

                        // enable deep clone when reading object from cache or not, default value is true.
                        EnableReadDeepClone = false,
                        // enable deep clone when writing object to cache or not, default value is false.
                        EnableWriteDeepClone = false,
                    };
                    // the max random second will be added to cache's expiration, default value is 120
                    config.MaxRdSecond = 120;
                    // whether enable logging, default is false
                    config.EnableLogging = false;
                    // mutex key's alive time(ms), default is 5000
                    config.LockMs = 5000;
                    // when mutex key alive, it will sleep some time, default is 300
                    config.SleepMs = 300;
                }, providerName1);
            });

builder.Services.AddDbContext<DataContext>((serviceProvider, optionsBuilder) =>
optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), NpgsqlDbContextOptionsBuilder =>
{
    NpgsqlDbContextOptionsBuilder
    .CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds)
    .EnableRetryOnFailure();
})
.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Авторизация тут для теста. Пример ввода: {token}",
        In = ParameterLocation.Header,
        Name = "x-access-token",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
      });

    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IVariantService, VariantService>();
builder.Services.AddScoped<IKitService, KitService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddCors(options => options.AddPolicy("corssus", policy =>
     policy.AllowCredentials().WithOrigins("http://localhost:3000", "https://kirikkostya.github.io").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.MapGet("api/addItems", async (DataContext db) =>
{
    StartUp startUp = new StartUp(db);
    await startUp.AddItems();
});

app.UseCors(options => options.WithOrigins("http://localhost:3000", "https://kirikkostya.github.io").AllowCredentials().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
