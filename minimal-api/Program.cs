
using MinimalApi.Infra.Db;
using MinimalApi.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Interface;
using MinimalApi.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Domain.Views;
using MinimalApi.Domain.Entities;

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

app.MapPost("admins/login", ([FromBody] LoginDTO loginDTO, iAdminService adminService) => {
    if(adminService.Login(loginDTO) != null){
        return Results.Ok("Login realizado com sucesso");
    }
    else {
        return Results.Unauthorized();
    }
}).WithTags("Admins");
#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, iVeiculoService veiculoService) => {
  

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
  
  veiculo.Nome = veiculoDTO.Nome;
  veiculo.Marca = veiculoDTO.Marca;
  veiculo.Ano = veiculoDTO.Ano;

  veiculoService.Update(veiculo);

  return Results.Ok(veiculo);

}).WithTags("Veiculos");


#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

#endregion