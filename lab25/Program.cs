using System;
using System.IO;

#region LOGGER (Factory Method + Singleton)

// Interface Logger
public interface ILogger
{
    void Log(string message);
}

// Console Logger
public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine("[Console] " + message);
    }
}

// File Logger
public class FileLogger : ILogger
{
    private string _filePath = "log.txt";

    public void Log(string message)
    {
        File.AppendAllText(_filePath, "[File] " + message + Environment.NewLine);
    }
}

// Factory Method
public abstract class LoggerFactory
{
    public abstract ILogger CreateLogger();
}

public class ConsoleLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger()
    {
        return new ConsoleLogger();
    }
}

public class FileLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger()
    {
        return new FileLogger();
    }
}

// Singletion logger manager
public class LoggerManager
{
    private static LoggerManager _instance;
    private ILogger _logger;

    private LoggerManager() { }

    public static LoggerManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new LoggerManager();
            return _instance;
        }
    }

    public void SetFactory(LoggerFactory factory)
    {
        _logger = factory.CreateLogger();
    }

    public void Log(string message)
    {
        _logger?.Log(message);
    }
}

#endregion

#region STRATEGY

// Interface Strategy
public interface IDataProcessorStrategy
{
    string Process(string data);
}

// Encrypt Strategy
public class EncryptDataStrategy : IDataProcessorStrategy
{
    public string Process(string data)
    {
        return "Encrypted(" + data + ")";
    }
}

// Compress Strategy
public class CompressDataStrategy : IDataProcessorStrategy
{
    public string Process(string data)
    {
        return "Compressed(" + data + ")";
    }
}

// Context
public class DataContext
{
    private IDataProcessorStrategy _strategy;

    public DataContext(IDataProcessorStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IDataProcessorStrategy strategy)
    {
        _strategy = strategy;
    }

    public string Execute(string data)
    {
        return _strategy.Process(data);
    }
}

#endregion

#region OBSERVER

// Publisher
public class DataPublisher
{
    public event Action<string> DataProcessed;

    public void PublishDataProcessed(string processedData)
    {
        DataProcessed?.Invoke(processedData);
    }
}

// Observer
public class ProcessingLoggerObserver
{
    public void OnDataProcessed(string data)
    {
        LoggerManager.Instance.Log("Processed data: " + data);
    }
}

#endregion

// Main

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("===== SCENARIO 1: FULL INTEGRATION =====");

        // Initialize LoggerManager with ConsoleLogger
        LoggerManager.Instance.SetFactory(new ConsoleLoggerFactory());

        // Strategy
        DataContext context = new DataContext(new EncryptDataStrategy());

        // Publisher
        DataPublisher publisher = new DataPublisher();

        // Observer
        ProcessingLoggerObserver observer = new ProcessingLoggerObserver();
        publisher.DataProcessed += observer.OnDataProcessed;

        string result1 = context.Execute("MyData");
        publisher.PublishDataProcessed(result1);

        Console.WriteLine();

        Console.WriteLine("===== SCENARIO 2: CHANGE LOGGER =====");

        // Change logger factory to FileLogger
        LoggerManager.Instance.SetFactory(new FileLoggerFactory());

        string result2 = context.Execute("MyDataAgain");
        publisher.PublishDataProcessed(result2);

        Console.WriteLine("Check log.txt file for file logging result.");
        Console.WriteLine();

        Console.WriteLine("===== SCENARIO 3: CHANGE STRATEGY =====");

        // Change strategy dynamically
        context.SetStrategy(new CompressDataStrategy());

        string result3 = context.Execute("AnotherData");
        publisher.PublishDataProcessed(result3);

        Console.WriteLine("Finished.");
    }
}
