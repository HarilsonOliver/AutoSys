using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Domain.Interface;

public interface iAdminService {
    Admin? Login(LoginDTO loginDTO);

    Admin Create(Admin admin);

    Admin? FindId(int id);

    List<Admin> All(int? pagina);
}