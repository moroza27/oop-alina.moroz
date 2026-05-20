using System;
using System.Collections.Generic;

namespace IndependentWork20
{
    // ПАТЕРН STRATEGY

    public interface IDataProcessorStrategy
    {
        void Process(string data);
    }

    public class CensorWordsStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            string censored = data.Replace("погане слово", "***");
            Console.WriteLine($"[Strategy: Censor] Оброблено: {censored}");
        }
    }

    public class TranslateMessageStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[Strategy: Translate] Переклад повідомлення '{data}' англійською...");
        }
    }

    public class StoreMessageStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[Strategy: Store] Повідомлення '{data}' надійно зашифровано та збережено в БД.");
        }
    }

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

        public void ExecuteProcessing(string data)
        {
            if (_strategy == null)
            {
                Console.WriteLine("Стратегію не встановлено!");
                return;
            }
            _strategy.Process(data);
        }
    }

    // ПАТЕРН OBSERVER

    public class DataPublisher
    {
        public event Action<string> DataProcessed;

        public void PublishDataProcessed(string data)
        {
            Console.WriteLine($"\n[Publisher] Сповіщення системи: Повідомлення '{data}' було оброблено. Розсилка спостерігачам...");
            
            DataProcessed?.Invoke(data);
        }
    }

    // Observers
    public class ConsoleOutputObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   -> [ConsoleObserver] Відображення в UI чату: {data}");
        }
    }

    public class MessageHistoryObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   -> [HistoryObserver] Запис у лог історії: {data} [{DateTime.Now:HH:mm:ss}]");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система обробки повідомлень чату ===\n");

            DataPublisher publisher = new DataPublisher();

            ConsoleOutputObserver consoleObserver = new ConsoleOutputObserver();
            MessageHistoryObserver historyObserver = new MessageHistoryObserver();

            publisher.DataProcessed += consoleObserver.OnDataProcessed;
            publisher.DataProcessed += historyObserver.OnDataProcessed;

            DataContext context = new DataContext(new CensorWordsStrategy());
            
            string message1 = "Привіт! Це погане слово, не читай його.";
            context.ExecuteProcessing(message1);
            publisher.PublishDataProcessed(message1);

            Console.WriteLine("\n------------------------------------------------\n");

            context.SetStrategy(new TranslateMessageStrategy());
            
            string message2 = "Як справи?";
            context.ExecuteProcessing(message2);
            publisher.PublishDataProcessed(message2);

            Console.WriteLine("\n------------------------------------------------\n");

            context.SetStrategy(new StoreMessageStrategy());
            
            string message3 = "Мій пароль: 123456";
            context.ExecuteProcessing(message3);
            
            publisher.DataProcessed -= consoleObserver.OnDataProcessed;
            Console.WriteLine("[Система] ConsoleObserver відписався від подій (приватне повідомлення).");
            
            publisher.PublishDataProcessed(message3);

            Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}