
using MinimalApi.Infra.Db;
using MinimalApi.DTOs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => {options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if(loginDTO.Email == "adm@test.com" && loginDTO.Senha == "123456"){
        return Results.Ok("Login realizado com sucesso");
    }
    else {
        return Results.Unauthorized();
    }
});


app.Run();

