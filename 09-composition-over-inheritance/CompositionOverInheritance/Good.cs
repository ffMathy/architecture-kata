namespace CompositionOverInheritance.Good;

// ✅ PART 1 - Compose behaviour from small pieces (Decorator + Composite).
//    N features = N classes, combined freely at runtime.

public interface INotifier
{
    void Send(string message);
}

public class EmailNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"  [Email] {message}");
}

public class SmsNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"  [SMS] {message}");
}

public class LoggingNotifier(INotifier inner, string name) : INotifier
{
    public void Send(string message)
    {
        Console.WriteLine($"  [Log] sending via {name}...");
        inner.Send(message);
    }
}

public class RetryingNotifier(INotifier inner, int attempts = 3) : INotifier
{
    public void Send(string message)
    {
        for (var attempt = 1; ; attempt++)
        {
            try { inner.Send(message); return; }
            catch when (attempt < attempts) { Console.WriteLine($"  [Retry] attempt {attempt} failed"); }
        }
    }
}

public class CompositeNotifier(params INotifier[] notifiers) : INotifier
{
    public void Send(string message)
    {
        foreach (var notifier in notifiers) notifier.Send(message);
    }
}

// ✅ PART 2 - Wrap instead of inherit: we depend only on the public contract,
//    so the inner collection's implementation can change freely.

public class TagCollection
{
    private readonly HashSet<string> _tags = [];
    public void Add(string tag) => _tags.Add(tag);
    public void AddRange(IEnumerable<string> tags) { foreach (var tag in tags) Add(tag); }
    public int Count => _tags.Count;
}

public class CountingTagCollection
{
    private readonly TagCollection _inner = new();

    public int AddAttempts { get; private set; }
    public int Count => _inner.Count;

    public void Add(string tag)
    {
        AddAttempts++;
        _inner.Add(tag);
    }

    public void AddRange(IEnumerable<string> tags)
    {
        var list = tags.ToList();
        AddAttempts += list.Count;
        _inner.AddRange(list); // _inner.Add is NOT our Add - no double counting
    }
}
