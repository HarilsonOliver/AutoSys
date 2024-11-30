
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

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<iAdminService, AdminService>();
builder.Services.AddScoped<iVeiculoService, VeiculoService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    options => {options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
var app = builder.Build();

#endregion

# region Home

app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

#endregion

#region Admnistradores

app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, iAdminService adminService) => {
    if(adminService.Login(loginDTO) != null){
        return Results.Ok("Login realizado com sucesso");
    }
    else {
        return Results.Unauthorized();
    }
}).WithTags("Admins");

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
}).WithTags("Admins");


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
  
  
}).WithTags("Admins");

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

}).WithTags("Admins");

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
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? page, iVeiculoService veiculoService) => {
  
  var veiculos = veiculoService.All(page);   
  return Results.Ok(veiculos);

}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromQuery] int id, iVeiculoService veiculoService) => {
  
  var veiculo = veiculoService.FindId(id);   

  if(veiculo == null){
    return Results.NotFound();
  }
  
  return Results.Ok(veiculo);

}).WithTags("Veiculos");

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

}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromQuery] int id, iVeiculoService veiculoService) => {
  
  var veiculo = veiculoService.FindId(id);   

  if(veiculo == null){
    return Results.NotFound();
  }
  

  veiculoService.Delete(veiculo);

  return Results.NoContent();

}).WithTags("Veiculos");


#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

#endregion