global using SharedCommonalities.ReturnModels.ReturnTypes;
global using gRPCStressTestingService.protos;
using gRPCStressTestingService.Implementations;
using gRPCStressTestingService.Services;
using SharedCommonalities.ObjectMapping;
using gRPCStressTestingService.Interfaces.Services;
using DbManagerWorkerService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConfigurationStuff.ServicesConfig;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using gRPCStressTestingService.DelayCalculations;
using gRPCStressTestingService.BackgroundServices;

namespace gRPCStressTestingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 1000 * 1024 * 1024;
               
                serverOptions.Listen(System.Net.IPAddress.Loopback, 5000, listenOptions =>
                {
                    listenOptions.UseHttps(); 
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });

            builder.Services.AddGrpc();

            var jwtAssertions = builder.Configuration.GetSection("Tokens");
            var tokenKey = jwtAssertions["Key"];
            var tokenIssuer = jwtAssertions["Issuer"];
            var tokenAudience = jwtAssertions["Audience"];

           builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = tokenIssuer,
                       ValidAudience = tokenAudience,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey))
                   };
               });


            // Add services to the container.
            builder.Services.AddGrpc(options =>
            {
                options.MaxReceiveMessageSize = 1000 * 1024 * 1024;
                options.MaxSendMessageSize = 1000 * 1024 * 1024;
            });

            ServiceConfig.AddSharedServices(builder.Services, builder.Configuration);

            builder.Services.AddScoped<IUnaryService, UnaryService>();
            builder.Services.AddScoped<UnaryImplementation>();

            builder.Services.AddScoped<DelayCalculation>();
            builder.Services.AddScoped<DatabaseTransportationService>();

       

            builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddScoped<ISessionService, SessionService>();

            builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();

            builder.Services.AddScoped<IClientInstanceService, ClientInstanceService>();

            builder.Services.AddScoped<IUtilitiesService, UtilitiesService>();
        

            builder.Services.AddScoped<IAdminService, AdminService>();

            builder.Services.AddScoped<IStreamingLatencyService, StreamingLatencyService>();

            builder.Services.AddScoped<IThroughputService, ThroughputService>();

            builder.Services.AddScoped<ObjectCreation>();
            builder.Services.AddScoped<delayCalcRepo>();

            builder.Services.AddHostedService<ThroughputReporter>();
         
            var app = builder.Build();
            
            app.UseRouting();

            app.MapGrpcService<UnaryImplementation>();
            app.MapGrpcService<AdminImplementation>();
            app.MapGrpcService<AccountImplementation>();
            app.MapGrpcService<AuthTokenImplementation>();
            app.MapGrpcService<SessionImplementation>();
            app.MapGrpcService<ClientInstanceImplementation>();
            app.MapGrpcService<UtilitiesImplementation>();
            app.MapGrpcService<StreamingImplementation>();
            app.MapGrpcService<ThroughputImplementation>();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}