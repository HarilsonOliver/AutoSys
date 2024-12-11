using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interface;
using MinimalApi.DTOs;
using MinimalApi.Infra.Db;

namespace MinimalApi.Domain.Services;

public class VeiculoService : iVeiculoService
{
    private readonly AppDbContext _context;
    public VeiculoService(AppDbContext context){

        _context =  context;

    }

    public List<Veiculo>? All(int? page = 1, string? nome = null, string? marca = null)
    {
       var query =_context.Veiculos.AsQueryable();
       if(!string.IsNullOrEmpty(nome)){
        query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));
       }

       int itensPorPagina = 10;

       if(page!= null){

        query = query.Skip(((int)page - 1 ) * itensPorPagina).Take(itensPorPagina);

       }

       
       return query.ToList();
    }

    public void Create(Veiculo veiculo)
    {
        _context.Veiculos.Add(veiculo);
        _context.SaveChanges();
    }

    public void Delete(Veiculo veiculo)
    {
        _context.Veiculos.Remove(veiculo);
        _context.SaveChanges();
    }

    public Veiculo? FindId(int id)
    {
        return _context.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Update(Veiculo veiculo)
    {
        _context.Veiculos.Update(veiculo);
        _context.SaveChanges();
    }
}