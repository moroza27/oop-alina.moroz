using System;

namespace IndependentWork19
{
    public interface ICache
    {
        void Save(string data);
    }

    // Реалізації ICache
    public class InMemoryCache : ICache
    {
        public void Save(string data)
        {
            Console.WriteLine($"[InMemory] Збереження даних у локальну пам'ять: {data}");
        }
    }

    public class RedisCache : ICache
    {
        public void Save(string data)
        {
            Console.WriteLine($"[Redis] Збереження даних у Redis-кластер: {data}");
        }
    }

    public class MemcachedCache : ICache
    {
        public void Save(string data)
        {
            Console.WriteLine($"[Memcached] Збереження даних у Memcached: {data}");
        }
    }

    // Абстрактна фабрика
    public abstract class CacheFactory
    {
        protected abstract ICache CreateCache();

        public void ProcessCache(string data)
        {
            ICache cache = CreateCache();
            cache.Save(data);
        }
    }

    // Фабрики
    public class InMemoryCacheFactory : CacheFactory
    {
        protected override ICache CreateCache()
        {
            return new InMemoryCache();
        }
    }

    public class RedisCacheFactory : CacheFactory
    {
        protected override ICache CreateCache()
        {
            return new RedisCache();
        }
    }

    public class MemcachedCacheFactory : CacheFactory
    {
        protected override ICache CreateCache()
        {
            return new MemcachedCache();
        }
    }

    // Клас CacheManager - Singleton
    public class CacheManager
    {
        private static CacheManager _instance;
        private CacheFactory _currentFactory;

        private CacheManager() { }

        public static CacheManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CacheManager();
                }
                return _instance;
            }
        }

        public void SetCacheFactory(CacheFactory factory)
        {
            _currentFactory = factory;
        }

        public void CacheData(string data)
        {
            if (_currentFactory != null)
            {
                _currentFactory.ProcessCache(data);
            }
            else
            {
                Console.WriteLine("Помилка: Фабрику кешування не встановлено!");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Робота системи кешування ===\n");

            CacheManager manager = CacheManager.Instance;

            manager.SetCacheFactory(new InMemoryCacheFactory());
            manager.CacheData("Налаштування користувача (ID: 101)");
            manager.CacheData("Тимчасові дані інтерфейсу");

            Console.WriteLine(); 

            manager.SetCacheFactory(new RedisCacheFactory());
            manager.CacheData("Сесія користувача #1024");
            manager.CacheData("Кошик товарів (Користувач ID: 101)");

            Console.WriteLine();

            manager.SetCacheFactory(new MemcachedCacheFactory());
            manager.CacheData("Результати пошукового запиту 'Патерни проєктування C#'");
            manager.CacheData("Список популярних статей за день");

            Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}