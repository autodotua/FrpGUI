using FrpGUI.Configs;
using FrpGUI.Models;
using FrpGUI.Service;
using FrpGUI.Service.Models;
using FrpGUI.Service.Services;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static bool swagger = true;

    private static WebApplication app;
    private static string cors = "cors";

    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = CreateBuilder(args);

        app = builder.Build();

        SettingApp(app);

        app.Run();
    }

    private static void AddConfigService(WebApplicationBuilder builder)
    {
        AppConfig config = new AppConfig();

        if (File.Exists(Path.Combine(AppContext.BaseDirectory, AppConfig.ConfigPath)))
        {
            try
            {
                config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllBytes(AppConfig.ConfigPath),
                    JsonHelper.GetJsonOptions(AppConfigSourceGenerationContext.Default));
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
        builder.Services.AddSingleton(config);
    }

    private static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            Args = args,
            ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default,
        });

        // Add services to the container.

        builder.Services.AddControllers(o =>
        {
            //不开这个，传入的参数有null（比如token）就会400 Bad Request
            o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            o.Filters.Add(app.Services.GetRequiredService<FrpGUIActionFilter>());

        })
            .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            o.JsonSerializerOptions.Converters.Add(new FrpConfigJsonConverter());
        });

        builder.Services.AddTransient<FrpGUIActionFilter>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(p =>
        {
            var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
            var xmlPath = Path.Combine(basePath, "FrpGUI.Service.xml");
            p.IncludeXmlComments(xmlPath);



            var scheme = new OpenApiSecurityScheme()
            {
                Description = "Authorization header",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Authorization"
                },
                Scheme = "oauth2",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            };
            p.AddSecurityDefinition("Authorization", scheme);
            var requirement = new OpenApiSecurityRequirement();
            requirement[scheme] = new List<string>();
            p.AddSecurityRequirement(requirement);
        });
        builder.Services.AddDbContext<FrpDbContext>(ServiceLifetime.Transient);
        builder.Services.AddSingleton<Logger>();
        builder.Services.AddSingleton<FrpProcessService>();
        builder.Services.AddHostedService<AppLifetimeService>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: cors,
                              policy =>
                              {
                                  policy.AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .AllowAnyOrigin();
                              });
        });

        builder.Host.UseWindowsService(c =>
        {
            c.ServiceName = "FrpGUI";
        });
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        AddConfigService(builder);
        return builder;
    }

    private static void SettingApp(WebApplication app)
    {
        app.Services.GetRequiredService<Logger>().Info("服务启动");
        if (swagger || app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        //app.UseWebSockets();
        app.UseHttpsRedirection();
        app.UseCors(cors);
        //app.UseAuthorization();
        app.MapControllers();
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppConfig))]
internal partial class AppConfigSourceGenerationContext : JsonSerializerContext
{
}
