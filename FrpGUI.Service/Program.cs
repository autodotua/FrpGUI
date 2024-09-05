using FrpGUI.Configs;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default,
});

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    o.JsonSerializerOptions.Converters.Add(new FrpConfigJsonConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWindowsService(c =>
{
    c.ServiceName = "FrpGUI";
});
Directory.SetCurrentDirectory(AppContext.BaseDirectory);
AddConfigService(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


static void AddConfigService(WebApplicationBuilder builder)
{
    AppConfig config = new AppConfig();

    if (File.Exists(Path.Combine(AppContext.BaseDirectory, AppConfig.ConfigPath)))
    {
        try
        {
            config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllBytes(AppConfig.ConfigPath), AppConfig.JsonOptions);
        }
        catch (Exception ex)
        {
            config = new AppConfig();
        }
    }
    if (config.FrpConfigs.Count == 0)
    {
        config.FrpConfigs.Add(new ServerConfig());
        config.FrpConfigs.Add(new ClientConfig());
    }
    builder.Services.AddSingleton<AppConfig>(config);
}