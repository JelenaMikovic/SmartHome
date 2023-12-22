using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nvt_back;
using System.Text;
using nvt_back.InfluxDB;
using nvt_back.Mqtt;
using nvt_back.Repositories;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services;
using nvt_back.Services.Interfaces;
using System.Configuration;
using Coravel;
using nvt_back.InfluxDB.Invocables;
using nvt_back.WebSockets;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.ClearProviders()));
}, ServiceLifetime.Scoped);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.Configure<EmailSettings>
   (options => builder.Configuration.GetSection("EmailSettings").Bind(options));


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
builder.Services.AddTransient<ICityRepository, CityRepository>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();
builder.Services.AddTransient<IAddressRepository, AddressRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddTransient<IDeviceRegistrationRepository, DeviceRegistrationRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPropertyService, PropertyService>();
builder.Services.AddTransient<IImageService, ImageService>();

builder.Services.AddTransient<IDeviceRegistrationService, DeviceRegistrationService>();
builder.Services.AddTransient<IDeviceOnlineStatusService, DeviceOnlineStatusService>();
builder.Services.AddTransient<IDeviceService, DeviceService>();
builder.Services.AddTransient<IDeviceRepository, DeviceRepository>();
builder.Services.Configure<MqttConfiguration>(builder.Configuration.GetSection("MqttConfiguration"));
builder.Services.AddScoped<IMqttClientService, MqttClientService>();
builder.Services.AddHostedService<MqttInitializationService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IDeviceStateService, DeviceStateService>();
builder.Services.AddTransient<IDeviceSimulatorInitializationService, DeviceSimulatorInitializationService>();
builder.Services.AddTransient<IDeviceDetailsService, DeviceDetailsService>();
builder.Services.AddSingleton<InfluxDBService>();
builder.Services.AddTransient<DeviceActivityCheckInvocable>();
builder.Services.AddTransient<PowerConsumptionInvocable>();
builder.Services.AddScheduler();

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

builder.Services.AddSignalR();

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

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ClaimsMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<DeviceHub>("/deviceHub");
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

/*var mqttClientService = app.Services.GetService<IMqttClientService>();

if (mqttClientService != null)
{
    mqttClientService.Connect();
} else
{
    Console.WriteLine("MqttClientService is null!");
}*/

app.Services.UseScheduler(scheduler =>
{
    scheduler
        .Schedule<DeviceActivityCheckInvocable>()
        .EverySeconds(30);
    scheduler.Schedule<PowerConsumptionInvocable>().EverySeconds(60);
});


app.Run();

