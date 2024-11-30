using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interface;
using MinimalApi.DTOs;
using MinimalApi.Infra.Db;

namespace MinimalApi.Domain.Services;

public class AdminService : iAdminService
{
    private readonly AppDbContext _context;
    public AdminService(AppDbContext context){

        _context =  context;

    }

    public List<Admin> All(int? page)
    {
        var query =_context.Admins.AsQueryable();
       
       int itensPorPagina = 10;

       if(page!= null){

        query = query.Skip(((int)page - 1 ) * itensPorPagina).Take(itensPorPagina);

       }

       
       return query.ToList();
    }

    

    public Admin Create(Admin admin)
    {

        _context.Admins.Add(admin);
        _context.SaveChanges();

        return admin;
    }

    public Admin? FindId(int id)
    {
        return _context.Admins.Where(v => v.Id == id).FirstOrDefault();
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        var adm= _context.Admins.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;

    }
}