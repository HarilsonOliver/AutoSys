using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Domain.Interface;

public interface iAdminService {
    Admin? Login(LoginDTO loginDTO);
}