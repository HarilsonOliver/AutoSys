
using MinimalApi.Infra.Db;
using MinimalApi.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Interface;
using MinimalApi.Domain.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<iAdminService, AdminService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    options => {options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, iAdminService adminService) => {
    if(adminService.Login(loginDTO) != null){
        return Results.Ok("Login realizado com sucesso");
    }
    else {
        return Results.Unauthorized();
    }
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

