using MinimalApi.Domain.Enuns;

namespace MinimalApi.Domain.Views;

public record AdminLogado {
    
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;

    public string Token { get; set; } = default!;

}