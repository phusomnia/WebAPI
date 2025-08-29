using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using WebAPI.Entities;

namespace WebAPI.Infrastructure.Data;

public partial class AppDbContext : DotnetContext
{
    private readonly string _connectionString;

    public AppDbContext(
        DbContextOptions<DotnetContext> options, 
        IConfiguration configuration
        ) : base(options)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }
    
    public ICollection<Dictionary<String, Object>> executeSqlRaw(String query, params Object[] parameters)
    {
        var dictResult = new Collection<Dictionary<String, Object>>();

        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        using var command = new MySqlCommand(query, connection);
        for (int i = 0; i < parameters.Length; ++i)
        {
            command.Parameters.AddWithValue($"{i}", parameters[i]);
        }

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var dict = Enumerable.Range(0, reader.FieldCount)
                .ToDictionary(
                    i => reader.GetName(i),
                    i => reader.IsDBNull(i) ? null : reader.GetValue(i)
                );
            dictResult.Add(dict);
        }

        return dictResult;
    }

    public async Task<ICollection<Dictionary<String, Object>>> executeSqlRawAsync(String query, params Object[] parameters)
    {
        var dictResult = new Collection<Dictionary<String, Object>>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(query, connection);
        for (int i = 0; i < parameters.Length; ++i)
        {
            command.Parameters.AddWithValue($"{i}", parameters[i]);
        }

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var dict = Enumerable.Range(0, reader.FieldCount)
                .ToDictionary(
                    i => reader.GetName(i),
                    i => reader.IsDBNull(i) ? null : reader.GetValue(i)
                );
            dictResult.Add(dict);
        }

        return dictResult;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString))
            .UseCamelCaseNamingConvention();
}