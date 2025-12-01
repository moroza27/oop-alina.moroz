using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading;

namespace IndependentWork13
{
    class Program
    {
        // Лічильники для імітації помилок
        private static int _apiCallAttempts = 0;
        private static int _dbCallAttempts = 0;
        private static int _queueSendAttempts = 0;

        // Сценарій 1: Виклик зовнішнього API з Retry ----------------
        public static string CallExternalApi(string url)
        {
            _apiCallAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Attempt {_apiCallAttempts}: Calling API {url}...");
            if (_apiCallAttempts <= 2) // Імітуємо 2 невдачі
            {
                throw new HttpRequestException($"API call failed for {url}");
            }
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] API call to {url} successful!");
            return "Data from API";
        }

        // Сценарій 2: Доступ до бази даних з Circuit Breaker ----------------
        public static string AccessDatabase()
        {
            _dbCallAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] DB Attempt {_dbCallAttempts}...");
            if (_dbCallAttempts <= 3) // Імітуємо 3 помилки
            {
                throw new Exception("Database temporarily unavailable");
            }
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] DB call successful!");
            return "DB Result";
        }

        // Сценарій 3: Відправка повідомлення в чергу з Timeout ----------------
        public static void SendMessageToQueue(string message)
        {
            _queueSendAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Queue Attempt {_queueSendAttempts} sending message...");
            // Імітуємо довге виконання при перших спробах
            if (_queueSendAttempts <= 2)
            {
                Thread.Sleep(5000); // 5 секунд, перевищує таймаут
            }
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Message sent successfully: {message}");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("--- Scenario 1: External API Call with Retry ---");
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetry(
                    3,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
                    });

            try
            {
                string apiResult = retryPolicy.Execute(() => CallExternalApi("https://api.example.com/data"));
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Final Result: {apiResult}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Operation failed after all retries: {ex.Message}");
            }

            Console.WriteLine("\n--- Scenario 2: Database Access with Circuit Breaker ---");
            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(2, TimeSpan.FromSeconds(10),
                onBreak: (ex, breakDelay) =>
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Circuit broken due to: {ex.Message}. Break for {breakDelay.TotalSeconds}s");
                },
                onReset: () =>
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Circuit reset. Operations can continue.");
                });

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    string dbResult = circuitBreakerPolicy.Execute(() => AccessDatabase());
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] DB call result: {dbResult}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] DB operation failed: {ex.Message}");
                }
                Thread.Sleep(1000);
            }

            Console.WriteLine("\n--- Scenario 3: Queue Message Sending with Timeout ---");
            var timeoutPolicy = Policy
                .Timeout(TimeSpan.FromSeconds(2), TimeoutStrategy.Pessimistic,
                onTimeout: (context, timespan, task) =>
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Operation timed out after {timespan.TotalSeconds}s");
                });

            try
            {
                timeoutPolicy.Execute(() => SendMessageToQueue("Test message"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Queue operation failed: {ex.Message}");
            }

            Console.WriteLine("\n--- End of Scenarios ---");
        }
    }
}
