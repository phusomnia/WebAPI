using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using WebAPI.Annotation;
using WebAPI.Features.AuthAPI.service.impl;

namespace WebAPI.Features.AuthAPI.service;

[Provider]
public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SALT_SIZE = 16;
    private const int HASH_SIZE = 20;
    private const int ITERATIONS = 1;
    
    public string hashPassword(string password)
    {
        Byte[] salt = new Byte[SALT_SIZE];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            ITERATIONS,
            HashAlgorithmName.SHA256,
            HASH_SIZE);

        Byte[] hashBytes = new Byte[SALT_SIZE + HASH_SIZE];
        Array.Copy(salt, 0, hashBytes, 0, SALT_SIZE);
        Array.Copy(hash, 0, hashBytes, SALT_SIZE, HASH_SIZE);

        return Convert.ToBase64String(hashBytes);
    }

    public bool verifyPassword(String providedPassword,String hashedPassword)
    {
        Byte[] hashBytes = Convert.FromBase64String(hashedPassword);
        Byte[] salt = new Byte[SALT_SIZE];

        Array.Copy(hashBytes, 0, salt, 0, SALT_SIZE);

        Byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            providedPassword,
            salt,
            ITERATIONS,
            HashAlgorithmName.SHA3_256,
            HASH_SIZE
        );

        for (int i = 0; i < HASH_SIZE; ++i)
        {
            if (hashBytes[i + SALT_SIZE] != hash[i])
            {
                return false;
            }
        }
        return true;
    }
}