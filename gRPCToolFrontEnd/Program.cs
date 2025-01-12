using Blazored.LocalStorage;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Components;
using gRPCToolFrontEnd.LocalStorage;
using gRPCToolFrontEnd.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MudBlazor.Services;
using Serilog;
using gRPCToolFrontEnd.Helpers;


namespace gRPCToolFrontEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
           
            // Add MudBlazor services
            builder.Services.AddMudServices();

            builder.Services.AddBlazoredLocalStorage();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddCircuitOptions(options =>
                {
                    options.DetailedErrors = true;
                  
                });


            builder.Services.AddGrpcClient<Accounts.AccountsClient>(o =>
            {
                o.Address = new Uri("https://localhost:5000");
            });

       
            builder.Services.AddScoped<AccountsService>();

            builder.Services.AddGrpcClient<ClientInstances.ClientInstancesClient>(o =>
            {
                o.Address = new Uri("https://localhost:5000");
            });

            builder.Services.AddScoped<ClientInstanceService>();

            builder.Services.AddGrpcClient<Sessions.SessionsClient>(o =>
            {
                o.Address = new Uri("https://localhost:5000");
            });

            builder.Services.AddScoped<SessionService>();

            builder.Services.AddGrpcClient<AuthTokens.AuthTokensClient>(o =>
            {
                o.Address = new Uri("https://localhost:5000");
            });

            builder.Services.AddScoped<AuthTokenService>();

            builder.Services.AddGrpcClient<Unary.UnaryClient>(o =>
            {
                o.Address = new Uri("https://localhost:5000");
            });

            builder.Services.AddGrpcClient<Utilities.UtilitiesClient>(o =>
            {
                o.Address = new Uri("https://localhost:5000");
            });

            builder.Services.AddGrpcClient<StreamingLatency.StreamingLatencyClient>(o =>
            {
                o.Address = new Uri("https://localhost:5000");
            });


            builder.Services.AddSingleton<UtilitiesService>();

            builder.Services.AddScoped<UnaryRequestService>();

            builder.Services.AddScoped<ClientHelper>();

            builder.Services.AddSingleton<AccountDetailsStore>();

            builder.Services.AddScoped<StreamingLatencyService>();

            var app = builder.Build();

            app.UseExceptionHandler("/Error");

            Console.WriteLine("APP HAS BEEN BUILT");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

           //app.UseAuthentication();
           //app.UseAuthorization();

            app.UseRouting();
            app.UseAntiforgery();
            

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
