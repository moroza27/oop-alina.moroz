# Code Smells та рефакторинг: практичний аналіз коду
### Вступ
Рефакторинг — це процес покращення внутрішньої структури коду без зміни його зовнішньої поведінки. Головним індикатором того, що код потребує втручання, є так звані "запахи коду" (Code Smells). Для практичного аналізу я взяла власний проєкт з попередньої лабораторної роботи. 

У цій програмі були реалізовані патерни Factory Method, Singleton, Strategy та Observer. Проте детальний аудит показав, що проєкт страждає від класичної проблеми розробників-початківців — "патерни заради патернів". Замість того, щоб вирішувати проблеми, архітектура створила нові. Нижче наведено розбір шести виявлених запахів коду та шляхи їх усунення.

Ось оновлений блок звіту з доданими фрагментами коду. Тепер усе максимально наочно:

### Аналіз виявлених Code Smells

#### 1. Надмірне проєктування
**Опис проблеми:** У коді створено цілу ієрархію `LoggerFactory` (абстрактний клас і дві конкретні фабрики: `ConsoleLoggerFactory`, `FileLoggerFactory`). Їхня єдина мета — повернути `new ConsoleLogger()` або `new FileLogger()`. Для такого тривіального створення об'єктів патерн Factory Method є абсолютно зайвим, він лише роздуває кодову базу.
**Техніка рефакторингу:** *Collapse Hierarchy* (Згортання ієрархії).
* **До:** Три класи фабрик спеціально для виклику конструктора.
```csharp
public abstract class LoggerFactory
{
    public abstract ILogger CreateLogger();
}
public class ConsoleLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger() => new ConsoleLogger();
}
public class FileLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger() => new FileLogger();
}
```
* **Після:** Видалення ієрархії фабрик. Створення логера напряму або використання вбудованого DI-контейнера.
```csharp
ILogger logger = new ConsoleLogger();
```

#### 2. Змінний глобальний стан
**Опис проблеми:** Клас `LoggerManager` реалізований як Singleton. Однак він має метод `SetFactory`, який дозволяє будь-кому і будь-коли змінити логер для всієї програми "на льоту". У багатопотоковому середовищі це призведе до хаосу: один потік перемкне логер на файловий, поки інший намагатиметься писати в консоль.
**Техніка рефакторингу:** *Remove Global State / Inject Dependency* (Позбавлення від глобального стану).
* **До:** Глобальний виклик `LoggerManager.Instance.SetFactory(...)`.
```csharp
public class LoggerManager
{
    public void SetFactory(LoggerFactory factory)
    {
        _logger = factory.CreateLogger(); 
    }
}
```
* **Після:** Повна відмова від Singleton. Передача конкретного об'єкта `ILogger` лише в ті класи, які його дійсно потребують, через конструктор (зберігаючи його у полі `readonly`).
```csharp
public class SomeDataService
{
    private readonly ILogger _logger; // Стан захищений від змін
    
    public SomeDataService(ILogger logger)
    {
        _logger = logger;
    }
}
```

#### 3. Прихована залежність
**Опис проблеми:** У класі `ProcessingLoggerObserver` метод `OnDataProcessed` жорстко прив'язаний до синглтона: `LoggerManager.Instance.Log(...)`. Дивлячись на конструктор цього класу, неможливо здогадатися, що йому для роботи потрібен логер. Це створює сильну зв'язність (Tight Coupling) та ускладнює модульне тестування.
**Техніка рефакторингу:** *Dependency Injection* (Впровадження залежностей).
* **До:** Прямий виклик глобального стану всередині методу.
```csharp
public class ProcessingLoggerObserver
{
    public void OnDataProcessed(string data)
    {
        LoggerManager.Instance.Log("Processed data: " + data);
    }
}
```
* **Після:** Передача залежності через конструктор.
```csharp
public class ProcessingLoggerObserver
{
    private readonly ILogger _logger;

    public ProcessingLoggerObserver(ILogger logger)
    {
        _logger = logger; 
    }

    public void OnDataProcessed(string data)
    {
        _logger.Log($"Processed data: {data}");
    }
}
```

#### 4. Лінивий клас
**Опис проблеми:** Клас `ProcessingLoggerObserver` створений виключно для того, щоб формально відповідати структурі патерну Observer. Він не має власного стану і містить лише один метод, який просто перенаправляє виклик до логера. Створення окремого класу для одного рядка коду — це Overengineering.
**Техніка рефакторингу:** *Inline Class / Use Lambda* (Вбудовування класу / Використання лямбда-виразу).
* **До:** Створення екземпляра `ProcessingLoggerObserver` та підписка його методу на подію.
```csharp
ProcessingLoggerObserver observer = new ProcessingLoggerObserver();
publisher.DataProcessed += observer.OnDataProcessed;
```
* **Після:** Видалення класу взагалі. Використання анонімної функції (лямбди).
```csharp
publisher.DataProcessed += (data) => _logger.Log($"Processed data: {data}");
```

#### 5. "Магічні рядки"
**Опис проблеми:** У класі `FileLogger` шлях до файлу жорстко закодований у полі класу: `private string _filePath = "log.txt";`. Через це неможливо використовувати цей логер для запису в різні файли.
**Техніка рефакторингу:** *Pass via Constructor* (Передача через конструктор).
* **До:** Жорстко задана константа всередині класу.
```csharp
public class FileLogger : ILogger
{
    private string _filePath = "log.txt";

    public void Log(string message)
    {
        File.AppendAllText(_filePath, "[File] " + message + Environment.NewLine);
    }
}
```
* **Після:** Передача шляху при створенні об'єкта.
```csharp
public class FileLogger : ILogger
{
    private readonly string _filePath;

    public FileLogger(string filePath = "log.txt") 
    {
        _filePath = filePath;
    }

    public void Log(string message)
    {
        File.AppendAllText(_filePath, $"[File] {message}\n");
    }
}
```

#### 6. Забутий слухач
**Опис проблеми:** У методі `Main` відбувається підписка на подію: `publisher.DataProcessed += observer.OnDataProcessed`. Проте в коді відсутня явна відписка від неї (`-=`). У мові C# події утримують жорсткі посилання на підписників. У реальних великих проєктах це найпоширеніша причина витоків пам'яті (Memory Leak), адже збирач сміття (Garbage Collector) не видалить об'єкт `observer`, доки існує `publisher`.
**Техніка рефакторингу:** *Explicit Unsubscribe* (Явна відписка).
* **До:** Тільки підписка `+=`.
```csharp
ProcessingLoggerObserver observer = new ProcessingLoggerObserver();
publisher.DataProcessed += observer.OnDataProcessed;
```
* **Після:** Додавання `publisher.DataProcessed -= observer.OnDataProcessed;` після завершення роботи з підписником.
```csharp
ProcessingLoggerObserver observer = new ProcessingLoggerObserver();
publisher.DataProcessed += observer.OnDataProcessed;

// ... виконання роботи програми ...

// Обов'язкова відписка, коли об'єкт більше не потрібен
publisher.DataProcessed -= observer.OnDataProcessed;
```

### Ризики рефакторингу без тестів
Головне правило рефакторингу: після зміни внутрішньої структури система має працювати точно так само, як і до неї. Робити це без модульних тестів (Unit Tests) украй небезпечно, адже ми працюємо "наосліп".

Розглянемо це на прикладі виправлення "магічних рядків". Я змінила конструктор `FileLogger`, щоб він приймав параметр `filePath`. Якщо в іншій частині великого проєкту або в іншому модулі створення цього логера відбувалося через рефлексію або IoC-контейнер, який очікував конструктор без параметрів, програма скомпілюється без помилок. Проте під час виконання (у Runtime) вона неминуче впаде з `MissingMethodException`. 

Якби проєкт був покритий автотестами, тест на ініціалізацію `FileLogger` впав би ще на етапі написання коду. Тести виконують роль "страйкувальної сітки", гарантуючи, що очищення коду від запахів не призведе до руйнування існуючої бізнес-логіки.

### Висновок
Аналіз власного коду продемонстрував, що використання патернів проєктування не гарантує якості архітектури. Патерни — це інструменти для вирішення конкретних проблем. Якщо проблеми немає, їх впровадження генерує Code Smells: надмірну складність, ліниві класи та приховані залежності. Чистий код повинен бути в першу чергу простим, зрозумілим і безпечним для майбутніх змін.