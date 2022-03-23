using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Livros.Commands;
using Application.Livros.Queries.ListarLivrosComDescricao;
using Application.Livros.Queries.ListarLivrosNomesLongo;
using Application.Livros.Queries.ListarTodosLivros;
using Application.Livros.Queries.ObterIdLivroPorNome;
using Application.Livros.Queries.ObterPrimeiroLivro;
using Application.Livros.Queries.ObterTitulosLivros;
using Domain.Livros.Entities;
using MandradeFrameworks.Autenticacao.Filters;
using MandradeFrameworks.Retornos.Controllers;
using MandradeFrameworks.Retornos.Models;
using MandradeFrameworks.SharedKernel.Models;
using MandradeFrameworks.SharedKernel.Usuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Api.Controllers.Livros
{
    [Route("livros/")]
    public class LivrosController : StandardController
    {
        public LivrosController(IMediator mediador) : base(mediador) { }

        // Método utilizando paginação
        [HttpGet]
        [Route("{pagina}/{quantidadeRegistros}")]
        public async Task<RetornoApi<ListaPaginada<Livro>>> ListarTodosLivros(int pagina, int quantidadeRegistros) =>
            await ProcessarSolicitacao(new ListarTodosLivrosQuery(pagina, quantidadeRegistros));

        // Método utilizando specification com paginação
        [HttpGet]
        [Route("nomes-longos/{pagina}/{quantidadeRegistros}")]
        public async Task<RetornoApi<ListaPaginada<Livro>>> ListarTodosLivrosComNomesLongos(int pagina, int quantidadeRegistros) =>
            await ProcessarSolicitacao(new ListarLivrosNomesLongoQuery(pagina, quantidadeRegistros));

        // Metodo apenas para exemplificar um simples endpoint
        [HttpGet]
        [Route("titulos")]
        public async Task<RetornoApi<IEnumerable<string>>> ObterTitulosLivros() =>
            await ProcessarSolicitacao(new ObterTitulosLivrosQuery());

        // Metodo que usa uma classe herdada do SqlRepositorio (via DI)
        [HttpGet]
        [Route("primeiro-livro")]
        public async Task<RetornoApi<Livro>> ObterPrimeiroLivro() =>
            await ProcessarSolicitacao(new ObterPrimeiroLivroQuery());

        // Metodo que usa uma classe herdada do SqlRepositorio (via DI) utilizando 
        // a consulta com specification pattern
        [HttpGet]
        [Route("livros-com-descricao")]
        public async Task<RetornoApi<IEnumerable<Livro>>> ListarLivrosComDescricao() =>
            await ProcessarSolicitacao(new ListarLivrosComDescricaoQuery());

        // Cadastro basico de um livro, com validacoes e uso de mensageria
        [HttpPost]
        [EndpointRestrito]
        public async Task<RetornoApi<bool>> CadastrarLivro([FromBody] CadastrarLivroCommand command) =>
            await ProcessarSolicitacao(command);

        // Método que realiza queries via Dapper. Esse método também utiliza logs.
        [HttpGet]
        [Route("{nome}/id")]
        public async Task<RetornoApi<Guid>> ObterIdLivroPorNome(string nome)
        {
            return await ProcessarSolicitacao(new ObterIdLivroPorNomeQuery(nome));
        }
    }
}