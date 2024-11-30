namespace MinimalApi.Domain.Views;

public struct Home{

    public string Message{
        get => "Bem vindo a API veículos - Minimal API";
    }

    public string Document{
        get => "/swagger";
    }
}