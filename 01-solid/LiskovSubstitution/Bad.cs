namespace LiskovSubstitution.Bad;

// ❌ "A square IS-A rectangle" is true in geometry, but not in code where
//    a Rectangle promises: "setting Width does not change Height".
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    public int Area => Width * Height;
}

public class Square : Rectangle
{
    // To stay a square we must break the parent's contract.
    public override int Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; }
    }

    public override int Height
    {
        get => base.Height;
        set { base.Width = value; base.Height = value; }
    }
}

// ❌ Second classic: a subtype that refuses part of the base contract.
public class FileStore
{
    protected readonly Dictionary<string, string> Files = new();
    public virtual void Save(string name, string content) => Files[name] = content;
    public string Read(string name) => Files[name];
}

public class ReadOnlyArchiveStore : FileStore
{
    public override void Save(string name, string content) =>
        throw new NotSupportedException("Archive is read-only!");
}

public static class Client
{
    // Written (and tested) against Rectangle. Perfectly reasonable code.
    public static void ResizeToBanner(Rectangle r)
    {
        r.Width = 10;
        r.Height = 2;
        var expected = 20;
        Console.WriteLine($"{r.GetType().Name,-10} expected area {expected}, got {r.Area}" +
                          (r.Area == expected ? "  ✅" : "  💥 LSP violated"));
    }

    public static void Backup(FileStore store)
    {
        try
        {
            store.Save("backup.txt", "important");
            Console.WriteLine($"{store.GetType().Name,-20} saved ✅");
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"{store.GetType().Name,-20} 💥 {ex.Message} - caller now needs 'if (store is ReadOnlyArchiveStore)'");
        }
    }
}
