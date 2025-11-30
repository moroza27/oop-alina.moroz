using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Lab7
{
    // Class FileProcessor
    public class FileProcessor
    {
        private int _attempts = 0;

        public string GetEmailTemplate(string path)
        {
            _attempts++;

            // Імітація перших 3 невдалих спроб
            if (_attempts <= 3)
            {
                throw new IOException($"[FileProcessor] Помилка читання файлу. Спроба #{_attempts}");
            }

            // Після 3-ї спроби — успіх
            return "Dear user,\nYour order has been processed successfully.\nRegards,\nSupport Team";
        }
    }

    // Class Networkclient
    public class NetworkClient
    {
        private int _attempts = 0;

        public void SendEmail(string recipient, string subject, string body)
        {
            _attempts++;

            // Імітація перших 2 невдалих спроб
            if (_attempts <= 2)
            {
                throw new HttpRequestException($"[NetworkClient] Мережева помилка. Спроба №{_attempts}");
            }

            // Після 2-ої спроби — успіх
            Console.WriteLine($"\nЛист успішно відправлено!");
            Console.WriteLine($"Кому: {recipient}");
            Console.WriteLine($"Тема: {subject}");
            Console.WriteLine($"Текст:\n{body}");
        }
    }

    // Class RetryHelper
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool> shouldRetry = null)
        {
            if (initialDelay == default)
                initialDelay = TimeSpan.FromSeconds(1);

            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    return operation(); // якщо успіх — повертаємо результат
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nПомилка: {ex.GetType().Name}\n   Повідомлення: {ex.Message}");
                    Console.ResetColor();

                    // Якщо не потрібно повторювати цю помилку — кидаємо далі
                    if (shouldRetry != null && !shouldRetry(ex))
                    {
                        Console.WriteLine("shouldRetry заборонив повторення. Операцію перервано.");
                        throw;
                    }

                    if (attempt >= retryCount)
                    {
                        Console.WriteLine("Вичерпано кількість спроб. Операцію перервано.");
                        throw;
                    }

                    // Експоненціальна затримка
                    var delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    Console.WriteLine($"Повторення через {delay.TotalSeconds:F1} сек...");

                    Thread.Sleep(delay);
                }
            }
        }

        // Версія без повернення значення
        public static void ExecuteWithRetry(
            Action operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool> shouldRetry = null)
        {
            ExecuteWithRetry<object>(() =>
            {
                operation();
                return null;
            }, retryCount, initialDelay, shouldRetry);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            // shouldRetry — повторюємо для IOException + HttpRequestException
            Func<Exception, bool> shouldRetry = (ex) =>
            {
                return ex is IOException || ex is HttpRequestException;
            };

            Console.WriteLine("========== 1. Читання шаблону листа ==========");

            string emailBody = RetryHelper.ExecuteWithRetry(
                () => fileProcessor.GetEmailTemplate("email.txt"),
                retryCount: 5,
                initialDelay: TimeSpan.FromMilliseconds(500),
                shouldRetry: shouldRetry
            );

            Console.WriteLine("\nФайл прочитано успішно!");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(emailBody);

            Console.WriteLine("\n========== 2. Відправка листа ==========");

            RetryHelper.ExecuteWithRetry(
                () => networkClient.SendEmail("user@example.com", "Order Confirmation", emailBody),
                retryCount: 4,
                initialDelay: TimeSpan.FromMilliseconds(400),
                shouldRetry: shouldRetry
            );
        }
    }
}
