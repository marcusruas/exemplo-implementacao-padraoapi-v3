using Microsoft.Extensions.DependencyInjection;
using MandradeFrameworks.Mensagens.Configuration;
using Microsoft.AspNetCore.Mvc;
using MandradeFrameworks.Retornos.Configuration;
using Microsoft.Extensions.Configuration;
using static MandradeFrameworks.Logs.Configuration.LogsConfiguration;
using MandradeFrameworks.Logs.Models;
using MandradeFrameworks.Autenticacao.Configuration;
using MandradeFrameworks.Tests.Models;
using MandradeFrameworks.Tests.Configuration;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Infrastructure.Livros;
using Infrastructure.Livros.Repository;

namespace Api.Configuration
{
    public static class DependencyInjection
    {
        private const string NOME_API = "API de exemplo";
        private const string VERSAO_API = "v1";

        public static void AdicionarPacotesFramework(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionStringTestes = configuration.GetConnectionString("Testes");
            string connectionStringLogs = configuration.GetConnectionString("Logs");
            var configuracoesLogs = new SQLLogsConfiguration(connectionStringLogs, "Logs_Livros");

            services.AdicionarMensageria();
            services.AdicionarAutenticacao();
            AdicionarLogsSQL(configuracoesLogs);
            services.AdicionarTestes(connectionStringTestes);
        }

        public static void AdicionarMiddlewares(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("Livros"), name: "Banco Principal")
                .AddSqlServer(configuration.GetConnectionString("Logs"), name: "Banco de Logs")
                .AddSqlServer(configuration.GetConnectionString("Testes"), name: "Banco de Testes");

            services.AddSwaggerGen(cnf =>
            {
                cnf.SwaggerDoc(VERSAO_API, new OpenApiInfo { Version = VERSAO_API, Title = NOME_API });
                cnf.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Header autenticacao via Json Web Tokens (JWT). insira abaixo o seu token da seguinte forma: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                cnf.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                        {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                    }
                });
            });
        }

        public static void AdicionarMiddlewaresAplicacao(this IApplicationBuilder application)
        {
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            application.UseHealthChecks("/status", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            application.UseSwagger();
            application.UseSwaggerUI();
        }

        public static void AdicionarPacotesFramework(this MvcOptions options)
        {
            options.AdicionarConfiguracoes();
        }

        public static void AdicionarRepositorios(this IServiceCollection services)
        {
            services.AddScoped<ILivrosRepositorio, LivrosRepositorio>();
        }
    }
}
