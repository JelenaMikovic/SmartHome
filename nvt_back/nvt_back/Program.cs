using Microsoft.EntityFrameworkCore;
using nvt_back;
using nvt_back.InfluxDB;
using nvt_back.Mqtt;
using nvt_back.Repositories;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services;
using nvt_back.Services.Interfaces;
using System.Configuration;
using Coravel;
using nvt_back.InfluxDB.Invocables;

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
builder.Services.AddTransient<IDeviceRegistrationRepository, DeviceRegistrationRepository>();
builder.Services.AddTransient<IPropertyService, PropertyService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IDeviceRegistrationService, DeviceRegistrationService>();
builder.Services.AddTransient<IDeviceActivationService, DeviceActivationService>();
builder.Services.AddTransient<IDeviceRepository, DeviceRepository>();
builder.Services.Configure<MqttConfiguration>(builder.Configuration.GetSection("MqttConfiguration"));
builder.Services.AddSingleton<MqttClientService>();

builder.Services.AddSingleton<InfluxDBService>();
builder.Services.AddTransient<DeviceActivityCheckInvocable>();
builder.Services.AddScheduler();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
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

app.UseCors();

app.UseRouting();

//app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

var mqttClientService = app.Services.GetService<MqttClientService>();

if (mqttClientService != null)
{
    mqttClientService.Connect();
} else
{
    Console.WriteLine("MqttClientService is null!");
}

app.Services.UseScheduler(scheduler =>
{
    scheduler
        .Schedule<DeviceActivityCheckInvocable>()
        .EverySeconds(20);
});


app.Run();
