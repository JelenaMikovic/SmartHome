using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nvt_back;
using System.Text;
using nvt_back.Repositories;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services;
using nvt_back.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.ClearProviders()));
}, ServiceLifetime.Transient);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddTransient<IPropertyRepository, PropertyRepository>();
builder.Services.AddTransient<ICityRepository, CityRepository>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();
builder.Services.AddTransient<IAddressRepository, AddressRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret_keysecret_keysecret_keysecret_key"))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["jwtToken"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("SuperAdmin", p => p.RequireRole("SUPERADMIN"));
    o.AddPolicy("Admin", p => p.RequireRole("ADMIN", "SUPERADMIN"));
    o.AddPolicy("User", p => p.RequireRole("USER"));
});

builder.Services.AddTransient<IPropertyRepository, PropertyRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<ILocationService, LocationService>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
           builder =>
           {
               builder.WithOrigins("http://localhost:4200") // Add the origin of your Angular app
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
           });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ClaimsMiddleware>();

app.UseCors();

app.UseRouting();
app.MapControllers();

app.Run();
