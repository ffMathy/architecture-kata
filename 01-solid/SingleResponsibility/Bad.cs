namespace SingleResponsibility.Bad;

public record InvoiceLine(string Description, int Quantity, decimal UnitPrice);

// ❌ One class, four reasons to change:
//    1. Accounting changes the VAT rules          -> CalculateTotal
//    2. Marketing wants a new invoice layout      -> Render
//    3. Ops moves invoices from disk to S3        -> Save
//    4. IT switches from SMTP to SendGrid         -> Send
// Every one of those people now edits (and risks breaking) the same file.
public class InvoiceService
{
    public void Process(string customerEmail, List<InvoiceLine> lines)
    {
        // --- business rules -----------------------------------------------
        decimal subtotal = 0;
        foreach (var line in lines)
            subtotal += line.Quantity * line.UnitPrice;
        var vat = subtotal * 0.25m;
        var total = subtotal + vat;

        // --- presentation -------------------------------------------------
        var text = "INVOICE\n";
        foreach (var line in lines)
            text += $"  {line.Quantity} x {line.Description} @ {line.UnitPrice:0.00}\n";
        text += $"  Subtotal: {subtotal:0.00}\n  VAT: {vat:0.00}\n  Total: {total:0.00}\n";

        // --- persistence --------------------------------------------------
        var path = Path.Combine(Path.GetTempPath(), $"invoice-{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, text);

        // --- infrastructure / messaging -----------------------------------
        Console.WriteLine($"[SMTP] Connecting to smtp.example.com:25 ...");
        Console.WriteLine($"[SMTP] Sending invoice to {customerEmail}:\n{text}");
    }
}
