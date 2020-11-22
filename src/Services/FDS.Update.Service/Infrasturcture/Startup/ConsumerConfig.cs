﻿namespace FDS.Update.Service.Infrasturcture.Startup
{
    using FDS.Common.Infrastructure.MessageQueue;
    using FDS.Common.Messages;
    using FDS.Update.Service.Consumers;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Security.Authentication;

    public static class ConsumerConfig
    {
        public static IServiceCollection AddConsumerServices(this IServiceCollection services, IHostEnvironment environment)
        {
            services
                 .AddTransient<StartUpdateConsumer>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var config = context.GetService<IRabbitMQConfiguration>();

                    if(environment.IsDevelopment())
                    {
                        cfg.Host(config.RabbitMQAddress, 5672, config.RabbitMQVHost, h =>
                        {
                            h.Password(config.RabbitMQPassword);
                        });
                    }
                    else
                    {
                        cfg.Host(config.RabbitMQAddress, 5671, config.RabbitMQVHost, h =>
                        {
                            h.Username(config.RabbitMQVHost);
                            h.Password(config.RabbitMQPassword);

                            h.UseSsl(s =>
                            {
                                s.Protocol = SslProtocols.Tls12;
                            });
                        });
                    }

                    cfg.ReceiveEndpoint(UrlBuilder.GetRoute(config.RabbitMQName, "StartUpdate"), e =>
                    {
                        e.Consumer<StartUpdateConsumer>(context);
                        e.PrefetchCount = 3;
                    });
                });
            });

            return services;
        }
    }
}
