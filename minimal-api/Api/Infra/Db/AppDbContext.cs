using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infra.Db;

public class AppDbContext : DbContext{

    private readonly IConfiguration _configAppSettings;
    public AppDbContext(IConfiguration configAppSettings){

        _configAppSettings = configAppSettings;

    }

    public DbSet<Admin> Admins{ get; set;} = default!;
    public DbSet<Veiculo> Veiculos{ get; set;} = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>().HasData(

            new Admin {
                Id = 1,
                Email = "admin@teste.com",
                Senha = "1234567",
                Perfil  = "Adm"
            }
        );
    }

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