using Bogus;
using Domain.Livros.Entities;
using MandradeFrameworks.Tests.UnitTests;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Livros.UnitTests.Domain
{
    public class LivroUnitTests : UnitTestsClass
    {
        [Fact]
        public void Livro_Instanciacao_DeveSerIgual()
        {
            var nome = _faker.Random.String(0, 100);
            var descricao = _faker.Random.String(0, 500);
            
            var livro = new Livro(nome, descricao);

            livro.Nome.ShouldBe(nome);
            livro.Descricao.ShouldBe(descricao);
        }

        [Fact]
        public void Livro_InstanciacaoSemDescricao_DeveSerIgual()
        {
            var nome = _faker.Random.String(0, 100);

            var livro = new Livro(nome, null);

            livro.Nome.ShouldBe(nome);
            livro.Descricao.ShouldBeNull();
        }
    }
}
