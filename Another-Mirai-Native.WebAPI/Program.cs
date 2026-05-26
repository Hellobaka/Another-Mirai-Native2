using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.WebAPI.Controllers;
using Another_Mirai_Native.WebAPI.Hubs;
using Another_Mirai_Native.WebAPI.Models;
using Another_Mirai_Native.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Security.Cryptography.X509Certificates;
using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models.MessageItem;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Another_Mirai_Native.WebAPI
{
    public class Program
    {
        private static readonly IJsonTypeInfoResolver MessageItemBaseResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                jsonTypeInfo =>
                {
                    if (jsonTypeInfo.Type == typeof(MessageItemBase))
                    {
                        jsonTypeInfo.PolymorphismOptions = new()
                        {
                            TypeDiscriminatorPropertyName = "messageItemType",
                            DerivedTypes =
                            {
                                new(typeof(Face),  (int)MessageItemType.Face),
                                new(typeof(BFace), (int)MessageItemType.Bface),
                                new(typeof(Image), (int)MessageItemType.Image),
                                new(typeof(Record),(int)MessageItemType.Record),
                                new(typeof(At),    (int)MessageItemType.At),
                                new(typeof(RPS),   (int)MessageItemType.Rps),
                                new(typeof(Shake), (int)MessageItemType.Shake),
                                new(typeof(Dice),  (int)MessageItemType.Dice),
                                new(typeof(Poke),  (int)MessageItemType.Poke),
                                new(typeof(RichContent), (int)MessageItemType.Rich),
                                new(typeof(Reply), (int)MessageItemType.Reply),
                                new(typeof(Text),  (int)MessageItemType.Text),
                            }
                        };
                    }
                }
            }
        };

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // TODO: For Local Test
                Task.Run(() => Entry.Main(args));
            }
            else
            {
                Entry.Main(args);
                return;
            }

            WebUIConfig.Instance.LoadConfig();

            var builder = WebApplication.CreateBuilder(args);

            var scheme = WebUIConfig.Instance.EnableHTTPS ? "https" : "http";
            builder.WebHost.UseUrls($"{scheme}://{WebUIConfig.Instance.ListenIP}:{WebUIConfig.Instance.ListenPort}");

            if (WebUIConfig.Instance.EnableHTTPS)
            {
                ConfigureHTTPS(builder.WebHost);
            }

            builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
                p.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(_ => true)));

            builder.Services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    o.JsonSerializerOptions.TypeInfoResolver = MessageItemBaseResolver;
                });

            AddOpenAPIService(builder);
            AddJWTAuthenticationService(builder);

            builder.Services.AddSingleton<EventBridgeService>();
            builder.Services.AddSingleton<DashboardService>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<EventBridgeService>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<DashboardService>());
            builder.Services.AddSignalR()
            .AddJsonProtocol(o =>
            {
                o.PayloadSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                o.PayloadSerializerOptions.TypeInfoResolver = MessageItemBaseResolver;
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(x =>
                {
                    x.PersistentAuthentication = true; // 刷新不丢失鉴权状态
                });
            }

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<MainHub>("/realtime");

            AddStaticFile(app);

            app.Run();
        }

        private static void AddOpenAPIService(WebApplicationBuilder builder)
        {
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Components ??= new();
                    document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                    {
                        ["Bearer"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            Description = "输入 JWT Token"
                        }
                    };
                    document.SecurityRequirements = [new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }];
                    return Task.CompletedTask;
                });
            });
        }

        private static void AddJWTAuthenticationService(WebApplicationBuilder builder)
        {
            builder.Services
              .AddAuthentication("Bearer")
              .AddJwtBearer(o =>
              {
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateAudience = false, // 忽略 Audience 验证
                      ValidateIssuer = false, // 忽略 Issuer 验证
                      ValidateLifetime = true, // 验证 Token 是否过期
                      IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                      {
                          // 每次验证时都从配置中获取密钥，确保使用最新的密钥进行验证
                          string key = AuthController.CurrentPassword;
                          var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                          return [signingKey];
                      }
                  };
                  o.Events = new JwtBearerEvents
                  {
                      OnMessageReceived = context =>
                      {
                          // SignalR 发起的 WebSocket 请求没法加 Header，token 走 Query String
                          var token = context.Request.Query["access_token"];
                          if (!string.IsNullOrEmpty(token) 
                            && (context.HttpContext.Request.Path.StartsWithSegments("/realtime") || context.HttpContext.Request.Path.StartsWithSegments("/api/cache")))
                          {
                              context.Token = token;
                          }

                          return Task.CompletedTask;
                      },
                      OnChallenge = context =>
                      {
                          // Skip the default logic.
                          context.HandleResponse();
                          context.Response.StatusCode = 401;
                          context.Response.ContentType = "application/json";
                          var result = System.Text.Json.JsonSerializer.Serialize(ApiResponse.Error(401, "未登录或登录失效"));
                          return context.Response.WriteAsync(result);
                      }
                  };
              });
            builder.Services.AddAuthorization();
        }

        private static void ConfigureHTTPS(ConfigureWebHostBuilder webHost)
        {
            if (File.Exists(WebUIConfig.Instance.CertificatePath) && File.Exists(WebUIConfig.Instance.CertificateKeyPath))
            {
                webHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        if (!TryLoadPEM(httpsOptions) && !TryLoadPFX(httpsOptions))
                        {
                            Console.Error.WriteLine("启用 HTTPS 失败，证书文件加载失败");
                        }
                    });
                });
            }
            else
            {
                Console.Error.WriteLine("启用 HTTPS 失败，证书文件不存在");
            }
        }

        private static bool TryLoadPFX(HttpsConnectionAdapterOptions options)
        {
            try
            {
                options.ServerCertificate = X509CertificateLoader.LoadPkcs12FromFile(
                    WebUIConfig.Instance.CertificatePath, WebUIConfig.Instance.CertificateKeyPath);
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"加载 PFX 证书失败：{ex.Message}");
                return false;
            }
        }

        private static bool TryLoadPEM(HttpsConnectionAdapterOptions options)
        {
            try
            {
                var pemCert = X509Certificate2.CreateFromPemFile(
                    WebUIConfig.Instance.CertificatePath, WebUIConfig.Instance.CertificateKeyPath);
                byte[] pfxBytes = pemCert.Export(X509ContentType.Pfx);
                options.ServerCertificate = X509CertificateLoader.LoadPkcs12(pfxBytes, null);
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"加载 PEM 证书失败：{ex.Message}");
                return false;
            }
        }

        private static void AddStaticFile(WebApplication app)
        {
            app.UseStaticFiles(new StaticFileOptions(new SharedOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "data", "image")),
                RequestPath = $"/external/{CachedFileType.Image}"
            }));
            app.UseStaticFiles(new StaticFileOptions(new SharedOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "data", "record")),
                RequestPath = $"/external/{CachedFileType.Record}"
            }));
            app.UseStaticFiles(new StaticFileOptions(new SharedOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "data", "video")),
                RequestPath = $"/external/{CachedFileType.Video}"
            }));
        }
    }
}
