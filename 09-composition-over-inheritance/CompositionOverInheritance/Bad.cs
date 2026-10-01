namespace CompositionOverInheritance.Bad;

// ❌ PART 1 - Class explosion.
//    Each new *combination* of features needs a new subclass.
//    Channels (Email, Sms) x Logging (on/off) x Retry (on/off) = 8 classes, and counting.

public abstract class Notifier
{
    public abstract void Send(string message);
}

public class EmailNotifier : Notifier
{
    public override void Send(string message) => Console.WriteLine($"  [Email] {message}");
}

public class SmsNotifier : Notifier
{
    public override void Send(string message) => Console.WriteLine($"  [SMS] {message}");
}

public class LoggingEmailNotifier : EmailNotifier
{
    public override void Send(string message)
    {
        Console.WriteLine("  [Log] sending email...");
        base.Send(message);
    }
}

public class LoggingSmsNotifier : SmsNotifier
{
    public override void Send(string message)
    {
        Console.WriteLine("  [Log] sending sms..."); // copy-paste of the logging logic
        base.Send(message);
    }
}

public class RetryingLoggingSmsNotifier : LoggingSmsNotifier
{
    public override void Send(string message)
    {
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try { base.Send(message); return; }
            catch when (attempt < 3) { }
        }
    }
}

// And "send to BOTH email and SMS"? Multiple inheritance isn't a thing. EmailAndSmsNotifier : ???

// ❌ PART 2 - The fragile base class problem.
//    A subclass depends on HOW the base class is implemented, not just WHAT it does.

public class TagCollection
{
    private readonly HashSet<string> _tags = [];

    public virtual void Add(string tag) => _tags.Add(tag);

    // Implementation detail: AddRange calls Add for each item.
    public virtual void AddRange(IEnumerable<string> tags)
    {
        foreach (var tag in tags) Add(tag);
    }

    public int Count => _tags.Count;
}

public class CountingTagCollection : TagCollection
{
    public int AddAttempts { get; private set; }

    public override void Add(string tag)
    {
        AddAttempts++;
        base.Add(tag);
    }

    public override void AddRange(IEnumerable<string> tags)
    {
        var list = tags.ToList();
        AddAttempts += list.Count;
        base.AddRange(list); // 💥 base calls our overridden Add -> counted twice
    }
}
