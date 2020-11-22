namespace FDS.Api.Infrastructure.Startup
{
    using FDS.Common.Infrastructure.MessageQueue;
    using FDS.Package.Service.Consumers;
    using MassTransit;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Security.Authentication;

    public static class MessageQueueConfig
    {
        public static IServiceCollection AddMessageQueueConfiguration(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddTransient<PackageUpdatedConsumer>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    if (environment.IsDevelopment())
                    {
                        cfg.Host(configuration.GetValue<string>("RabbitMQ:Address"), h =>
                        {
                            h.Password(configuration.GetValue<string>("RabbitMQ:Password"));
                        });

                    }
                    else
                    {
                        cfg.Host(configuration.GetValue<string>("RabbitMQ:Address"), 5671, configuration.GetValue<string>("RabbitMQ:VHost"), h =>
                        {
                            h.Username(configuration.GetValue<string>("RabbitMQ:VHost"));
                            h.Password(configuration.GetValue<string>("RabbitMQ:Password"));

                            h.UseSsl(s =>
                            {
                                s.Protocol = SslProtocols.Tls12;
                            });
                        });
                    }

                    cfg.ReceiveEndpoint(UrlBuilder.GetRoute(configuration.GetValue<string>("RabbitMQ:Name"), "PackageUpdated"), e =>
                    {
                        e.Consumer<PackageUpdatedConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
