using System;
using System.Text;

namespace IndependentWork23
{
    // ПАТЕРН ADAPTER

    public interface IMessageSender
    {
        void Send(string message);
    }

    public class LegacyMessageQueue
    {
        public void Enqueue(byte[] message)
        {
            Console.WriteLine($"[LegacyQueue] Повідомлення додано до черги. Розмір: {message.Length} байт.");
        }
    }

    public class LegacyMessageAdapter : IMessageSender
    {
        private readonly LegacyMessageQueue _adaptee;

        public LegacyMessageAdapter(LegacyMessageQueue adaptee)
        {
            _adaptee = adaptee;
        }

        public void Send(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            _adaptee.Enqueue(bytes);
        }
    }

    // ПАТЕРН FACADE

    public class MessageFormatter
    {
        public string Format(string message) => $"*** {message.Trim().ToUpper()} ***";
    }

    public class MessageValidator
    {
        public bool Validate(string message) => !string.IsNullOrWhiteSpace(message);
    }

    public class MessageSenderSubsystem
    {
        public void Send(string message)
        {
            Console.WriteLine($"[Subsystem Sender] Відправка: {message}");
        }
    }

    public class MessageProcessingFacade
    {
        private readonly MessageFormatter _formatter = new MessageFormatter();
        private readonly MessageValidator _validator = new MessageValidator();
        private readonly MessageSenderSubsystem _sender = new MessageSenderSubsystem();

        public void ProcessAndSend(string message)
        {
            Console.WriteLine("[Facade] Початок обробки повідомлення...");
            if (_validator.Validate(message))
            {
                string formattedMsg = _formatter.Format(message);
                _sender.Send(formattedMsg);
                Console.WriteLine("[Facade] Повідомлення успішно оброблено та відправлено.");
            }
            else
            {
                Console.WriteLine("[Facade] Помилка: повідомлення не пройшло валідацію.");
            }
        }
    }

    // ПАТЕРН PROXY

    public interface IMessageBroker
    {
        void Publish(string topic, string message);
        void Subscribe(string topic);
    }

    public class RealMessageBroker : IMessageBroker
    {
        public void Publish(string topic, string message)
        {
            Console.WriteLine($"[RealBroker] Публікація в топік '{topic}': {message}");
        }

        public void Subscribe(string topic)
        {
            Console.WriteLine($"[RealBroker] Підписка на топік '{topic}' оформлена.");
        }
    }

    public class LoggingMessageBrokerProxy : IMessageBroker
    {
        private RealMessageBroker _realBroker;

        private RealMessageBroker Broker
        {
            get
            {
                if (_realBroker == null)
                {
                    _realBroker = new RealMessageBroker();
                }
                return _realBroker;
            }
        }

        public void Publish(string topic, string message)
        {
            Console.WriteLine($"[Proxy Log - {DateTime.Now:HH:mm:ss}] Спроба публікації повідомлення в '{topic}'...");
            Broker.Publish(topic, message);
            Console.WriteLine($"[Proxy Log] Публікація успішна.\n");
        }

        public void Subscribe(string topic)
        {
            Console.WriteLine($"[Proxy Log - {DateTime.Now:HH:mm:ss}] Запит на підписку до '{topic}'...");
            Broker.Subscribe(topic);
            Console.WriteLine($"[Proxy Log] Підписка успішна.\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Робота структурних патернів ===\n");

            // --- Демонстрація Adapter ---
            Console.WriteLine("--- Демонстрація Adapter ---");
            LegacyMessageQueue legacyQueue = new LegacyMessageQueue();
            IMessageSender adapter = new LegacyMessageAdapter(legacyQueue);
            
            adapter.Send("Привіт, стара черга!");
            Console.WriteLine();

            // --- Демонстрація Facade ---
            Console.WriteLine("---Демонстрація Facade ---");
            MessageProcessingFacade facade = new MessageProcessingFacade();
            
            facade.ProcessAndSend("   нове повідомлення для фасаду   ");
            Console.WriteLine();

            // --- Демонстрація Proxy ---
            Console.WriteLine("--- Демонстрація Proxy ---");
            IMessageBroker brokerProxy = new LoggingMessageBrokerProxy();
            
            brokerProxy.Subscribe("news_channel");
            brokerProxy.Publish("news_channel", "Сьогодні вивчили структурні патерни!");

            Console.WriteLine("Роботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}