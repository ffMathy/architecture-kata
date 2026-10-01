namespace InterfaceSegregation.Good;

public record Document(string Title);

// ✅ Small, role-based interfaces. Clients depend only on what they use.
public interface IPrinter { void Print(Document doc); }
public interface IScanner { void Scan(Document doc); }
public interface IFax { void Fax(Document doc, string number); }
public interface IStapler { void Staple(Document doc); }

public class OfficeMegaPrinter3000 : IPrinter, IScanner, IFax, IStapler
{
    public void Print(Document doc) => Console.WriteLine($"[Mega] Printing {doc.Title}");
    public void Scan(Document doc) => Console.WriteLine($"[Mega] Scanning {doc.Title}");
    public void Fax(Document doc, string number) => Console.WriteLine($"[Mega] Faxing {doc.Title} to {number}");
    public void Staple(Document doc) => Console.WriteLine($"[Mega] Stapling {doc.Title}");
}

// Honest: it only claims what it can actually do.
public class CheapInkjet : IPrinter
{
    public void Print(Document doc) => Console.WriteLine($"[Inkjet] Printing {doc.Title}");
}

public class ReportPrinter(IPrinter printer)
{
    public void PrintQuarterlyReport() => printer.Print(new Document("Q3 report"));
}

public class ArchiveScanner(IScanner scanner)
{
    public void ScanInvoices() => scanner.Scan(new Document("Invoices"));
}
