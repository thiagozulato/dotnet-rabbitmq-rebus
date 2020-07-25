using System.Reflection;
using Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

namespace Pagamento.Api
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
                return configure.Transport(transport => transport.UseRabbitMq("amqp://shop:shop_123@localhost:5672", "pagamento_fila")
                                                                 .Declarations(true, true, true)
                                                                 .ClientConnectionName("Pagamento.Api")
                                                                 .ExchangeNames("shop_direct", "shop_topics"))
                                .Routing(routing =>
                                {
                                    routing.TypeBased()
                                           .MapAssemblyOf<PagamentoAprovadoEvent>("pagamento_fila");
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
                await sub.Subscribe<PagamentoIniciadoEvent>();
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
