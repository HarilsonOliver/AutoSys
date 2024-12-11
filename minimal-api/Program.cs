
using MinimalApi.Infra.Db;
using MinimalApi.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Interface;
using MinimalApi.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Domain.Views;
using MinimalApi.Domain.Entities;
using System.Runtime.CompilerServices;
using MinimalApi.Domain.Enuns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option => {
  option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  
}).AddJwtBearer(option =>{
  option.TokenValidationParameters = new TokenValidationParameters{

    ValidateLifetime = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
    ValidateIssuer = false,
    ValidateAudience = false

  };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<iAdminService, AdminService>();
builder.Services.AddScoped<iVeiculoService, VeiculoService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>{
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Insira o token JWT aqui"
  });
  options.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme{
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      new string [] {}
    }
  });
});



builder.Services.AddDbContext<AppDbContext>(
    options => {options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
var app = builder.Build();

#endregion

# region Home

app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

#endregion

#region Admnistradores

string GerarTokenJwt(Admin admin){
  if(string.IsNullOrEmpty(key)) return string.Empty;
  
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>(){

      new Claim("Email", admin.Email),
      new Claim("Perfil", admin.Perfil),
      new Claim(ClaimTypes.Role, admin.Perfil)
    };

    var token = new JwtSecurityToken(
      claims: claims,
      expires: DateTime.Now.AddDays(1),
      signingCredentials: credentials
    );  

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, iAdminService adminService) => {
    
    var adm = adminService.Login(loginDTO);
    if(adm != null){
        string token = GerarTokenJwt(adm);
        
        return Results.Ok(new AdminLogado{
          Email = adm.Email,
          Perfil = adm.Perfil,
          Token = token
        });
    }
    else {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Admins");

app.MapGet("/admins", ([FromQuery] int? page, iAdminService adminService) => {
    var adms = new List<AdminView>();
    var admins = adminService.All(page);
    foreach(var adm in admins){
      adms.Add(new AdminView{
        Id = adm.Id,
        Email = adm.Email,
        Perfil = adm.Perfil      
      });
    }
    return Results.Ok(adms);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles  = "Adm"})
.WithTags("Admins");


app.MapPost("/admins", ([FromBody] AdminDTO adminDTO, iAdminService adminService) => {
  var validation = new ValidationErrors{
    Messages = new List<string>()
  };

  if(string.IsNullOrEmpty(adminDTO.Email)){
    validation.Messages.Add("O campo Email não pode ser vazio");
  }
  if(string.IsNullOrEmpty(adminDTO.Senha)){
    validation.Messages.Add("O campo Senha não pode ser vazio");
  }
  if(adminDTO.Perfil == null){
    validation.Messages.Add("O campo Perfil não pode ser vazio");
  }
  if(validation.Messages.Count > 0){
    return Results.BadRequest(validation);
  }
      var admin = new Admin {
      Email = adminDTO.Email,
      Senha = adminDTO.Senha,
      Perfil = adminDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };
    adminService.Create(admin); 
    return Results.Created($"/admins/{admin.Id}", new AdminView{
      Id = admin.Id,
      Email = admin.Email,
      Perfil = admin.Perfil
    });
  
  
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles  = "Adm"})
.WithTags("Admins");

app.MapGet("/admins/{id}", ([FromQuery] int id, iAdminService adminService) => {
  
  var admin = adminService.FindId(id);   

  if(admin == null){
    return Results.NotFound();
  }  
  return Results.Ok(new AdminView{
    Id = admin.Id,
    Email = admin.Email,
    Perfil = admin.Perfil
  });

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles  = "Adm"})
.WithTags("Admins");

#endregion

#region Veiculos

ValidationErrors validaDTO(VeiculoDTO veiculoDTO){

    var validation = new ValidationErrors{
        Messages = new List<string>()
    };

  if(string.IsNullOrEmpty(veiculoDTO.Nome)){
    validation.Messages.Add("O Nome não pode ser vazio");
  }
  if(string.IsNullOrEmpty(veiculoDTO.Marca)){
    validation.Messages.Add("A Marca não pode estar em branco");
  }
  if(veiculoDTO.Ano < 1950){
    validation.Messages.Add("Veículo muito antigo, somente aceito anos superiores a 1950!");
  }

  return validation;

}
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, iVeiculoService veiculoService) => {
  var validation = validaDTO(veiculoDTO);
  if(validation.Messages.Count > 0){
    return Results.BadRequest(validation);
  }
  var veiculo = new Veiculo {
    Nome = veiculoDTO.Nome,
    Marca = veiculoDTO.Marca,
    Ano = veiculoDTO.Ano
  };
  veiculoService.Create(veiculo);
  return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles  = "Adm,Editor"})
.WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? page, iVeiculoService veiculoService) => {
  
  var veiculos = veiculoService.All(page);   
  return Results.Ok(veiculos);

}).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromQuery] int id, iVeiculoService veiculoService) => {
  
  var veiculo = veiculoService.FindId(id);   

  if(veiculo == null){
    return Results.NotFound();
  }
  
  return Results.Ok(veiculo);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles  = "Adm,Editor"})
.WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromQuery] int id, VeiculoDTO veiculoDTO,iVeiculoService veiculoService) => {
  
  
  var veiculo = veiculoService.FindId(id);   
  if(veiculo == null){
    return Results.NotFound();
  }

  var validation = validaDTO(veiculoDTO);
  if(validation.Messages.Count > 0){
    return Results.BadRequest(validation);
  }

  
  
  veiculo.Nome = veiculoDTO.Nome;
  veiculo.Marca = veiculoDTO.Marca;
  veiculo.Ano = veiculoDTO.Ano;

  veiculoService.Update(veiculo);

  return Results.Ok(veiculo);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles  = "Adm"})
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromQuery] int id, iVeiculoService veiculoService) => {
  
  var veiculo = veiculoService.FindId(id);   

  if(veiculo == null){
    return Results.NotFound();
  }
  

  veiculoService.Delete(veiculo);

  return Results.NoContent();

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles  = "Adm"})
.WithTags("Veiculos");


#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

#endregion