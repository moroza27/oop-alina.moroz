using System;
using System.Collections.Generic;

// Interface Strategy
public interface INumericOperationStrategy
{
    double Execute(double value);
}

public class SquareOperationStrategy : INumericOperationStrategy
{
    public double Execute(double value)
    {
        return value * value;
    }
}

// Strategy
public class CubeOperationStrategy : INumericOperationStrategy
{
    public double Execute(double value)
    {
        return value * value * value;
    }
}

public class SquareRootOperationStrategy : INumericOperationStrategy
{
    public double Execute(double value)
    {
        return Math.Sqrt(value);
    }
}

public class NumericProcessor
{
    private INumericOperationStrategy _strategy;

    public NumericProcessor(INumericOperationStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(INumericOperationStrategy strategy)
    {
        _strategy = strategy;
    }

    public double Process(double input)
    {
        return _strategy.Execute(input);
    }
}

public class ResultPublisher
{
    public event Action<double, string> ResultCalculated;

    public void PublishResult(double result, string operationName)
    {
        ResultCalculated?.Invoke(result, operationName);
    }
}

// Observer
public class ConsoleLoggerObserver
{
    public void OnResultCalculated(double result, string operationName)
    {
        Console.WriteLine($"Operation: {operationName}, Result: {result}");
    }
}

// Observer
public class HistoryLoggerObserver
{
    public List<string> History { get; private set; } = new List<string>();

    public void OnResultCalculated(double result, string operationName)
    {
        string record = $"Operation: {operationName}, Result: {result}";
        History.Add(record);
    }
}

// Observer
public class ThresholdNotifierObserver
{
    private double _threshold;

    public ThresholdNotifierObserver(double threshold)
    {
        _threshold = threshold;
    }

    public void OnResultCalculated(double result, string operationName)
    {
        if (result > _threshold)
        {
            Console.WriteLine($"Result exceeded threshold ({_threshold})!");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Створюю початкову стратегію (квадрат)
        INumericOperationStrategy squareStrategy = new SquareOperationStrategy();
        NumericProcessor processor = new NumericProcessor(squareStrategy);

        // Створюю publisher
        ResultPublisher publisher = new ResultPublisher();

        // Створюю спостерігачів
        ConsoleLoggerObserver consoleObserver = new ConsoleLoggerObserver();
        HistoryLoggerObserver historyObserver = new HistoryLoggerObserver();
        ThresholdNotifierObserver thresholdObserver = new ThresholdNotifierObserver(50);

        // Підписка на подію
        publisher.ResultCalculated += consoleObserver.OnResultCalculated;
        publisher.ResultCalculated += historyObserver.OnResultCalculated;
        publisher.ResultCalculated += thresholdObserver.OnResultCalculated;

        // Обробка числа 5 (квадрат)
        double result1 = processor.Process(5);
        publisher.PublishResult(result1, "Square");

        // Змінюю стратегію на куб
        processor.SetStrategy(new CubeOperationStrategy());
        double result2 = processor.Process(4);
        publisher.PublishResult(result2, "Cube");

        // Змінюю стратегію на квадратний корінь
        processor.SetStrategy(new SquareRootOperationStrategy());
        double result3 = processor.Process(16);
        publisher.PublishResult(result3, "Square Root");

        Console.WriteLine("\nHistory:");
        foreach (var record in historyObserver.History)
        {
            Console.WriteLine(record);
        }
    }
}
