namespace WebAPI.Features.AuthAPI.service.impl;

public interface IPasswordHasher
{
    String hashPassword(String password);
    Boolean verifyPassword(String hashedPassword, String providedPassword);
}