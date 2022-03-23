using Api;
using Domain.Livros.Entities;
using Infrastructure.DBContexts;
using MandradeFrameworks.Retornos.Models;
using MandradeFrameworks.SharedKernel.Models;
using MandradeFrameworks.Tests.Integration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Livros.IntegrationTests
{
    public class LivrosControllerIntegrationTests : IntegrationTestsController<Startup, LivrosContext>
    {
        public LivrosControllerIntegrationTests(ApplicationTests<Startup, LivrosContext> app)
        : base(app, SetupDb()) { }

        [Fact]
        public async Task ListarTodosLivros_DeveRetornarSucesso()
        {
            var urlRequest = $"{UrlBase}/1/10";
            var request = await Cliente.GetAsync(urlRequest);
            var resposta = await DesserializarResposta<ListaPaginadaModel<Livro>>(request);

            resposta.Sucesso.ShouldBeTrue();
            resposta.Dados.Itens.ShouldNotBeEmpty();
        }

        protected static Func<LivrosContext, Task> SetupDb()
        {
            return async context =>
            {
                await context.Livros.AddAsync(new Livro("IT - A Coisa", "Livro sobre palhaço"));
                await context.SaveChangesAsync();
            };
        }

        protected override string UrlControllerBase() => "livros";
    }
}