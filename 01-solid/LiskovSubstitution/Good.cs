namespace LiskovSubstitution.Good;

// ✅ Model what clients actually rely on. Immutable shapes cannot have
//    their invariants broken behind the client's back.
public interface IShape
{
    int Area { get; }
}

public record Rectangle(int Width, int Height) : IShape
{
    public int Area => Width * Height;
}

public record Square(int Side) : IShape
{
    public int Area => Side * Side;
}

// ✅ Split the contract so no subtype has to refuse a promise.
public interface IReadableStore
{
    string Read(string name);
}

public interface IWritableStore : IReadableStore
{
    void Save(string name, string content);
}

public class FileStore : IWritableStore
{
    private readonly Dictionary<string, string> _files = new();
    public void Save(string name, string content) => _files[name] = content;
    public string Read(string name) => _files[name];
}

public class ReadOnlyArchiveStore(IReadOnlyDictionary<string, string> files) : IReadableStore
{
    public string Read(string name) => files[name];
}

public static class Client
{
    public static void PrintArea(IShape shape) =>
        Console.WriteLine($"{shape,-30} area {shape.Area}  ✅");

    // The compiler now prevents passing a read-only store here.
    public static void Backup(IWritableStore store)
    {
        store.Save("backup.txt", "important");
        Console.WriteLine($"{store.GetType().Name,-20} saved ✅");
    }
}
