using Grpc.Net.Client;
using gRPCToolFrontEnd.Components;
using gRPCToolFrontEnd.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MudBlazor.Services;
using Serilog;


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

            var app = builder.Build();

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

           // app.UseAuthentication();
           //app.UseAuthorization();

            app.UseRouting();
            app.UseAntiforgery();
            

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
