using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.Routing.TransportMessages;
using Rebus.ServiceProvider;
using Rebus.Extensions;
using Rebus.Serialization.Custom;
using Rebus.Serialization.Json;
using Rebus.Retry.Simple;
using Messages;

namespace Venda.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRebus(configure =>
            {
                return configure.Transport(transport => transport.UseRabbitMq("amqp://shop:shop_123@localhost:5672", "venda_fila")
                                                                 .Declarations(true, true, true)
                                                                 .ClientConnectionName("Venda.Api")
                                                                 .ExchangeNames("shop_direct", "shop_topics"))
                                .Routing(routing =>
                                {
                                    routing.TypeBased()
                                           .MapAssemblyOf<PagamentoIniciadoEvent>("venda_fila");
                                })
                                .Options(options =>
                                {
                                    options.SetNumberOfWorkers(1);
                                    options.SetMaxParallelism(1);
                                    options.SimpleRetryStrategy(maxDeliveryAttempts: 5);
                                    options.SetBusName("shop_bus");
                                });
            });

            services.AutoRegisterHandlersFromAssembly(Assembly.GetExecutingAssembly());

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.UseRebus(async sub =>
            {
                await sub.Subscribe<PagamentoAprovadoEvent>();
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
