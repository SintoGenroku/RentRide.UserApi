using IdentityServer4.AccessTokenValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Users.Api.Consumers;
using Users.Api.Extensions;
using Refit;
using RentRide.AuthenticationApi.Models;
using Serilog;
using Users.Api.Handlers;
using Users.Data;
using Users.Data.Repositories;
using Users.Data.Repositories.Abstracts;
using Users.Refit;
using Users.Services;
using Users.Services.Abstracts;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;


services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Users.API", 
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "SintoGenroku",
            Email = "borozda.a.s@gmail.com",
            Url = new Uri("https://github.com/SintoGenroku")
        }
    });
                
    var filePath = Path.Combine(AppContext.BaseDirectory, "Users.Api.xml");

    options.IncludeXmlComments(filePath);
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

services.AddTransient<AuthorizationHandler>();

services.AddRefitClient<IAuthenticationApi>()
    .ConfigureHttpClient(config =>
{
    var stringUrl = builder.Configuration.GetConnectionString("GatewayURL");
    config.BaseAddress = new Uri(stringUrl);
});

services.AddDbContext<UsersDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IAuthenticationService, AuthenticationService>();

services.AddAutoMapper(configuration => { configuration.AddMaps(typeof(Program).Assembly); });

services.AddMassTransit(c =>
{
    c.AddConsumer<AuthenticationConsumer>();
    
    c.UsingRabbitMq((context, config) =>
    {
        config.ReceiveEndpoint("user-userApi-queue", e =>
        {
            e.Bind<UserQueue>();
            e.ConfigureConsumer<AuthenticationConsumer>(context);
        });
    });
});

services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
    {
        options.Authority = "http://localhost:5035";
        options.RequireHttpsMetadata = false;
        options.ApiName = "Users.Api";
        options.ApiSecret = "users-secret";
    });

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

app.UseCustomExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Users.API v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();