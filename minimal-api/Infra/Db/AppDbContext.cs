using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infra.Db;

public class AppDbContext : DbContext{

    private readonly IConfiguration _configAppSettings;
    public AppDbContext(IConfiguration configAppSettings){

        _configAppSettings = configAppSettings;

    }

    public DbSet<Admin> Admins{ get; set;} = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured){
            var stringConnection = _configAppSettings.GetConnectionString("Default")?.ToString();
            if(!string.IsNullOrEmpty(stringConnection)){

                optionsBuilder.UseSqlServer(stringConnection);

            }
        }

        
    }

}