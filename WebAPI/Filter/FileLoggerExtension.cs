using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;
using System.Text;

namespace WebAPI.Filter;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;

    public FileLoggerProvider(string filePath)
    {
        _filePath = filePath;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(this, categoryName);
    }

    public void Dispose() { }

    internal void WriteMessage(string message)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        
        var dir = parentDir(basePath, 3);
        Console.WriteLine($"Dir: {dir}");
        
        var fullPath = Path.Combine(dir, _filePath);
        Console.WriteLine($"Fullpath: {fullPath}");
        
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        
        File.AppendAllText(fullPath, message);
    }

    public string parentDir(string path, int level = 1)
    {
        var result = path;
        for (int i = 0; i <= level; i++)
        {
            result = Directory.GetParent(result)!.FullName;
        }

        return result;
    }
}

public class FileLogger : ILogger
{
    private readonly FileLoggerProvider _provider;
    private readonly string _categoryName;

    public FileLogger(
        FileLoggerProvider provider,
        string categoryName
        )
    {
        _provider = provider;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = new StringBuilder();
        message.AppendLine($"[{DateTime.UtcNow:u}] [{logLevel}] {_categoryName}: {formatter(state, exception)}");
        if (exception != null)
        {
            message.AppendLine(exception.ToString());
        }
        _provider.WriteMessage(message.ToString());
    }
}

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath)
    {
        builder.Services.AddSingleton<ILoggerProvider>(new FileLoggerProvider(filePath));
        return builder;
    }
}
