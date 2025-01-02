global using SharedCommonalities.ReturnModels.ReturnTypes;
global using gRPCStressTestingService.protos;
using gRPCStressTestingService.Implementations;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.Services;
using SharedCommonalities.TimeStorage;
using DelayCalculationWorkerService;
using DelayCalculationWorkerService.Service;
using DbManagerWorkerService;
using DbManagerWorkerService.Services;
using DbManagerWorkerService.Interfaces;

using DbManagerWorkerService.Interfaces.DataContext;
using DbManagerWorkerService.Repos;
using ClientManagementWorkerService.Interfaces;
using ClientManagementWorkerService.Services;
using ClientManagementWorker;
using SharedCommonalities.Storage;
using SharedCommonalities.UsefulFeatures;
using SharedCommonalities.ObjectMapping;
using gRPCStressTestingService.Interfaces.Services;
using DbManagerWorkerService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ConfigurationStuff;
using ConfigurationStuff.ServicesConfig;
using Microsoft.AspNetCore.Server.Kestrel.Core;


namespace gRPCStressTestingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
               
                serverOptions.Limits.MaxRequestBodySize = int.MaxValue;

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
                options.MaxReceiveMessageSize = 100 * 1024 * 1024;
            });

            ServiceConfig.AddSharedServices(builder.Services, builder.Configuration);

            builder.Services.AddScoped<IUnaryService, UnaryService>();
            builder.Services.AddScoped<UnaryImplementation>();
            builder.Services.AddSingleton<DelayCalculations>(); 
            builder.Services.AddHostedService<DbManagerWorker>();

            builder.Services.AddSingleton<IClientManagementService, ClientManagementService>();
            //builder.Services.AddHostedService<ClientManagerWorker>();

           // builder.Services.AddScoped<ICommunicationDelayRepo, CommunicationDelayRepo>();
           // builder.Services.AddScoped<ICommunicationDelayService, CommunicationDelayService>();

            builder.Services.AddScoped<IAccountService, AccountService>();
           // builder.Services.AddScoped<IAccountRepo, AccountRepo>();

            builder.Services.AddScoped<ISessionService, SessionService>();
            //builder.Services.AddScoped<ISessionRepo, SessionRepo>();

            builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
          //  builder.Services.AddScoped<IAuthTokenRepo, AuthTokenRepo>();

            builder.Services.AddScoped<IClientInstanceService, ClientInstanceService>();
           // builder.Services.AddScoped<IClientInstanceRepo, ClientInstanceRepo>();  

            builder.Services.AddScoped<ICommunicationDelayService, CommunicationDelayService>();

            builder.Services.AddScoped<IAdminService, AdminService>();

            builder.Services.AddScoped<ObjectCreation>();
         
            builder.Services.AddHostedService<DelayWorker>();

            var app = builder.Build();
            
            app.UseRouting();

            app.MapGrpcService<UnaryImplementation>();
            app.MapGrpcService<AdminImplementation>();
            app.MapGrpcService<AccountImplementation>();
            app.MapGrpcService<AuthTokenImplementation>();
            app.MapGrpcService<SessionImplementation>();
            app.MapGrpcService<ClientInstanceImplementation>();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}