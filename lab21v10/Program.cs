using System;

namespace lab21v10
{
    // Стратегії комісій 
    public interface IExchangeStrategy
    {
        decimal CalculateCommission(decimal amountInUah);
    }

    public class StandardExchangeStrategy : IExchangeStrategy
    {
        public decimal CalculateCommission(decimal amountInUah)
        {
            // 1% + 5 грн
            return (amountInUah * 0.01m) + 5;
        }
    }

    public class CardExchangeStrategy : IExchangeStrategy
    {
        public decimal CalculateCommission(decimal amountInUah)
        {
            // 2% + 10 грн
            return (amountInUah * 0.02m) + 10;
        }
    }

    public class CryptoExchangeStrategy : IExchangeStrategy
    {
        public decimal CalculateCommission(decimal amountInUah)
        {
            // 0.5%, мінімум 20 грн
            decimal commission = amountInUah * 0.005m;
            return commission < 20 ? 20 : commission;
        }
    }

    // Демонстраціz OCP
    public class VipExchangeStrategy : IExchangeStrategy
    {
        public decimal CalculateCommission(decimal amountInUah)
        {
            return 0; // VIP без комісії
        }
    }

    // Стратегія Фабрика (
    public static class ExchangeStrategyFactory
    {
        public static IExchangeStrategy CreateStrategy(string type)
        {
            switch (type?.ToLower())
            {
                case "standard": return new StandardExchangeStrategy();
                case "card": return new CardExchangeStrategy();
                case "crypto": return new CryptoExchangeStrategy();
                case "vip": return new VipExchangeStrategy();
                default: throw new ArgumentException("Невідомий тип обміну");
            }
        }
    }

    // Сервіс обміну 
    public class ExchangeService
    {
        // Курс валют
        private const decimal RateUSD = 41.5m;
        private const decimal RateEUR = 45.0m;

        public void PerformExchange(decimal amountUah, string currencyCode, IExchangeStrategy strategy)
        {
            // Розрахунок комісії 
            decimal commission = strategy.CalculateCommission(amountUah);
            
            decimal amountToConvert = amountUah - commission;

            if (amountToConvert <= 0)
            {
                Console.WriteLine("Сума замала для покриття комісії!");
                return;
            }

            // Конвертування по курсу
            decimal finalAmount = 0;
            decimal rate = 1;

            switch (currencyCode.ToUpper())
            {
                case "USD":
                    rate = RateUSD;
                    finalAmount = amountToConvert / RateUSD;
                    break;
                case "EUR":
                    rate = RateEUR;
                    finalAmount = amountToConvert / RateEUR;
                    break;
                default:
                    Console.WriteLine("Вибачте, такої валюти ми не маємо.");
                    return;
            }

            // Вивід чеку
            Console.WriteLine("\n--- КВИТАНЦІЯ ---");
            Console.WriteLine($"Ви дали:       {amountUah} UAH");
            Console.WriteLine($"Комісія:       {commission} UAH (знято перед обміном)");
            Console.WriteLine($"До обміну:     {amountToConvert} UAH");
            Console.WriteLine($"Курс {currencyCode}:      {rate}");
            Console.WriteLine($"ОТРИМАНО:      {Math.Round(finalAmount, 2)} {currencyCode}");
            Console.WriteLine("-----------------\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ExchangeService service = new ExchangeService();

            while (true)
            {
                try
                {
                    Console.WriteLine("Введіть валюту, яку хочете купити (USD або EUR) або 'exit':");
                    string currency = Console.ReadLine() ?? ""; 

                    if (currency.ToLower() == "exit") break;

                    Console.WriteLine("Оберіть метод оплати (Standard, Card, Crypto, VIP):");
                    string type = Console.ReadLine() ?? ""; 

                    Console.Write("Скільки гривень міняємо? ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        Console.WriteLine("Некоректна сума!");
                        continue;
                    }

                    IExchangeStrategy strategy = ExchangeStrategyFactory.CreateStrategy(type);

                    service.PerformExchange(amount, currency, strategy);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
            }
        }
    }
}