using System;

// ЧАСТИНА 1. Погана реалізація
namespace Lab23_Bad
{
    public class WeaponSystem
    {
        public void Attack() => Console.WriteLine("Вжух! Удар мечем.");
    }

    public class MedicalKit
    {
        public void Heal() => Console.WriteLine("Пшш... Здоров'я відновлено.");
    }

    public class DialogueManager
    {
        public void Speak() => Console.WriteLine("Привіт, мандрівниче!");
    }

    public class HeroAction
    {
        // Порушення DIP
        private WeaponSystem _weapon = new WeaponSystem();
        private MedicalKit _medKit = new MedicalKit();
        private DialogueManager _dialogue = new DialogueManager();

        public void PerformAttack() => _weapon.Attack();
        public void PerformHeal() => _medKit.Heal();
        public void PerformSpeak() => _dialogue.Speak();
    }
}

// ЧАСТИНА 2. Хороша реалізація
namespace Lab23_Good
{
    // Інтерфейси
    public interface IWeapon { void Attack(); }
    public interface IHealer { void Heal(); }
    public interface ITalker { void Speak(); }

    // Реалізація
    public class Sword : IWeapon
    {
        public void Attack() => Console.WriteLine("Вжух! Удар залізним мечем.");
    }

    public class Bow : IWeapon
    {
        public void Attack() => Console.WriteLine("Тиць! Стріла полетіла.");
    }

    public class SmallMedKit : IHealer
    {
        public void Heal() => Console.WriteLine("Використано бинт (+10 HP).");
    }

    public class NpcDialogue : ITalker
    {
        public void Speak() => Console.WriteLine("NPC: Ласкаво просимо в наше село.");
    }

    // Герой (DIP + DI)
    public class SuperHero
    {
        private readonly IWeapon _weapon;
        private readonly IHealer _healer;
        private readonly ITalker _talker;

        public SuperHero(IWeapon weapon, IHealer healer, ITalker talker)
        {
            _weapon = weapon;
            _healer = healer;
            _talker = talker;
        }

        public void Action()
        {
            _weapon.Attack();
            _healer.Heal();
            _talker.Speak();
        }
    }

    // ISP приклад
    public class SilentWarrior
    {
        private readonly IWeapon _weapon;

        public SilentWarrior(IWeapon weapon)
        {
            _weapon = weapon;
        }

        public void Fight()
        {
            Console.Write("(Мовчить і б'є) -> ");
            _weapon.Attack();
        }
    }
}

namespace Lab23_App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Запуск поганого варіанту
            Console.WriteLine("=== BAD IMPLEMENTATION ===");
            Lab23_Bad.HeroAction badHero = new Lab23_Bad.HeroAction();
            badHero.PerformAttack();
            badHero.PerformHeal();
            badHero.PerformSpeak();

            Console.WriteLine("\n-----------------------------\n");

            // Запуск хорошого варіанту
            Console.WriteLine("=== GOOD IMPLEMENTATION (SOLID) ===");

            // Створюємо деталі
            Lab23_Good.IWeapon sword = new Lab23_Good.Sword();
            Lab23_Good.IWeapon bow = new Lab23_Good.Bow();
            Lab23_Good.IHealer kit = new Lab23_Good.SmallMedKit();
            Lab23_Good.ITalker chat = new Lab23_Good.NpcDialogue();

            // Герой з мечем
            Console.WriteLine("--- Super Hero (з мечем) ---");
            Lab23_Good.SuperHero goodHero = new Lab23_Good.SuperHero(sword, kit, chat);
            goodHero.Action();

            // Герой з луком
            Console.WriteLine("\n--- Super Hero (з луком) ---");
            Lab23_Good.SuperHero archer = new Lab23_Good.SuperHero(bow, kit, chat);
            archer.Action();

            // Німий воїн
            Console.WriteLine("\n--- Silent Warrior (Тільки б'ється) ---");
            Lab23_Good.SilentWarrior berzerk = new Lab23_Good.SilentWarrior(sword);
            berzerk.Fight();

            Console.ReadKey();
        }
    }
}