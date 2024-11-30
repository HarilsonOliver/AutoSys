using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Domain.Interface;

public interface iVeiculoService {
    List<Veiculo>? All(int? page = 1, string? nome = null, string? marca = null);
    Veiculo? FindId(int id);
    void Create(Veiculo veiculo);

    void Update(Veiculo veiculo);

    void Delete(Veiculo veiculo);

}