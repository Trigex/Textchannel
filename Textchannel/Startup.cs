using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Blazor.Http;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TextbinChannel.Services;
using Textchannel.Services;

namespace Textchannel
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(GetAppConfiguration());
            services.AddSingleton<ToastService>();
            services.AddSingleton<Api>();
            services.AddSingleton<AppState>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY")))
                WebAssemblyHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;

            app.AddComponent<App>("app");
        }

        public AppConfiguration GetAppConfiguration()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("config.json"))
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                return JsonConvert.DeserializeObject<AppConfiguration>(reader.ReadToEnd());
            }
        }
    }
}
