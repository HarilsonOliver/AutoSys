using MinimalApi.Domain.Enuns;

namespace MinimalApi.Domain.Views;

public record AdminView {
    

    
    public int Id {get ; set; } = default!;
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;

}