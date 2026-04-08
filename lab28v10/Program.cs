using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace lab28v10
{
    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Location
    {
        public string City { get; set; }
        public string Address { get; set; }
    }

    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public Location EventLocation { get; set; }
        public List<Participant> Participants { get; set; } = new List<Participant>();
    }

    public interface IEventRepository
    {
        void Add(Event ev);
        IEnumerable<Event> GetAll();
        Event GetById(int id);
        Task SaveToFileAsync(string filename);
        Task LoadFromFileAsync(string filename);
    }

    public class EventRepository : IEventRepository
    {
        private List<Event> _events = new List<Event>();
        private readonly JsonSerializerOptions _jsonOptions;

        public EventRepository()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true, 
                PropertyNameCaseInsensitive = true 
            };
        }

        public void Add(Event ev)
        {
            _events.Add(ev);
        }

        public IEnumerable<Event> GetAll()
        {
            return _events; 
        }

        public Event GetById(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id); 
        }

        public async Task SaveToFileAsync(string filename)
        {
            using FileStream createStream = File.Create(filename);
            await JsonSerializer.SerializeAsync(createStream, _events, _jsonOptions);
        }

        public async Task LoadFromFileAsync(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"[Увага] Файл {filename} не знайдено. Репозиторій порожній.");
                return;
            }

            using FileStream openStream = File.OpenRead(filename);
            _events = await JsonSerializer.DeserializeAsync<List<Event>>(openStream, _jsonOptions) ?? new List<Event>();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string filename = "events_data.json";

            var repo = new EventRepository();

            var event1 = new Event
            {
                Id = 1,
                Title = "C# Developers Meetup",
                Date = new DateTime(2026, 5, 20, 18, 0, 0),
                EventLocation = new Location { City = "Київ", Address = "вул. Хрещатик, 1" },
                Participants = new List<Participant>
                {
                    new Participant { Id = 1, Name = "Іван Іванов", Email = "ivan@example.com" },
                    new Participant { Id = 2, Name = "Олена Петрівна", Email = "olena@example.com" }
                }
            };

            var event2 = new Event
            {
                Id = 2,
                Title = "Global IT Conference",
                Date = new DateTime(2026, 9, 10, 10, 0, 0),
                EventLocation = new Location { City = "Львів", Address = "вул. Стрийська, 199" },
                Participants = new List<Participant>
                {
                    new Participant { Id = 3, Name = "Андрій Шевченко", Email = "andriy@example.com" }
                }
            };

            repo.Add(event1);
            repo.Add(event2);

            Console.WriteLine("\nЗберігаємо дані у файл JSON...");
            await repo.SaveToFileAsync(filename);
            Console.WriteLine("Дані успішно збережено!");

            Console.WriteLine("\nЗавантажуємо дані з файлу в новий репозиторій...");
            var newRepo = new EventRepository();
            await newRepo.LoadFromFileAsync(filename);
            Console.WriteLine("Дані успішно завантажено!");

            Console.WriteLine("\nСписок всіх івентів:");
            foreach (var ev in newRepo.GetAll())
            {
                Console.WriteLine($"Івент: {ev.Title} (ID: {ev.Id})");
                Console.WriteLine($"Дата: {ev.Date:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"Локація: {ev.EventLocation?.City}, {ev.EventLocation?.Address}");
                Console.WriteLine("Учасники:");
                foreach (var p in ev.Participants)
                {
                    Console.WriteLine($"  - {p.Name} ({p.Email})");
                }
                Console.WriteLine(new string('-', 40));
            }

            Console.WriteLine("\nПошук івенту за ID = 2:");
            var foundEvent = newRepo.GetById(2);
            if (foundEvent != null)
            {
                Console.WriteLine($"Знайдено: {foundEvent.Title} у місті {foundEvent.EventLocation.City}");
            }
            else
            {
                Console.WriteLine("Івент не знайдено.");
            }
        }
    }
}