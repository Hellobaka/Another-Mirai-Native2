using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Another_Mirai_Native.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
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
                          string key = WebUIConfig.Instance.Password;
                          var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                          return [signingKey];
                      }
                  };
                  o.Events = new JwtBearerEvents
                  {
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
            builder.Services.AddSwaggerGen(x =>
            {
                // Swagger UI 中添加 JWT 认证支持
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
