using FrpGUI.Configs;
using FrpGUI.Models;
using FrpGUI.Services;
using FrpGUI.WebAPI.Models;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.OpenApi.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace FrpGUI.WebAPI;

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
            var xmlPath = Path.Combine(basePath, "FrpGUI.WebAPI.xml");
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
        builder.Services.AddSingleton<LoggerBase, Logger>();
        builder.Services.AddSingleton<FrpProcessCollection>();
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

        AppConfig config = AppConfigBase.Get<AppConfig>();
        builder.Services.AddSingleton(config);

        return builder;
    }

    private static void SettingApp(WebApplication app)
    {
        app.Services.GetRequiredService<LoggerBase>().Info("服务启动");
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