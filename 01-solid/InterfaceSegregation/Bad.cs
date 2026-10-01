namespace InterfaceSegregation.Bad;

public record Document(string Title);

// ❌ A "fat" interface: every implementer must support every capability,
//    and every client depends on methods it never calls.
public interface IMultiFunctionDevice
{
    void Print(Document doc);
    void Scan(Document doc);
    void Fax(Document doc, string number);
    void Staple(Document doc);
}

public class OfficeMegaPrinter3000 : IMultiFunctionDevice
{
    public void Print(Document doc) => Console.WriteLine($"[Mega] Printing {doc.Title}");
    public void Scan(Document doc) => Console.WriteLine($"[Mega] Scanning {doc.Title}");
    public void Fax(Document doc, string number) => Console.WriteLine($"[Mega] Faxing {doc.Title} to {number}");
    public void Staple(Document doc) => Console.WriteLine($"[Mega] Stapling {doc.Title}");
}

// The cheap home printer is forced to lie about what it can do.
public class CheapInkjet : IMultiFunctionDevice
{
    public void Print(Document doc) => Console.WriteLine($"[Inkjet] Printing {doc.Title}");
    public void Scan(Document doc) => throw new NotImplementedException();
    public void Fax(Document doc, string number) => throw new NotImplementedException();
    public void Staple(Document doc) => throw new NotImplementedException();
}

public class ReportPrinter(IMultiFunctionDevice device)
{
    // Only needs Print... but recompiles/redeploys when Fax's signature changes,
    // and a test double has to stub four methods.
    public void PrintQuarterlyReport() => device.Print(new Document("Q3 report"));
}

public class ArchiveScanner(IMultiFunctionDevice device)
{
    public void ScanInvoices() => device.Scan(new Document("Invoices"));
}
