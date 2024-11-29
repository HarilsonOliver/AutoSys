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
    public Admin? Login(LoginDTO loginDTO)
    {
        var adm= _context.Admins.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;

    }
}