namespace SingleResponsibility.Good;

public record InvoiceLine(string Description, int Quantity, decimal UnitPrice);

public record Invoice(IReadOnlyList<InvoiceLine> Lines, decimal Subtotal, decimal Vat, decimal Total);

// ✅ Each class has exactly one reason to change, owned by one "actor".

/// <summary>Owned by accounting: VAT and totals.</summary>
public class InvoiceCalculator
{
    private const decimal VatRate = 0.25m;

    public Invoice Calculate(IReadOnlyList<InvoiceLine> lines)
    {
        var subtotal = lines.Sum(l => l.Quantity * l.UnitPrice);
        var vat = subtotal * VatRate;
        return new Invoice(lines, subtotal, vat, subtotal + vat);
    }
}

/// <summary>Owned by marketing/design: how an invoice looks.</summary>
public class InvoiceRenderer
{
    public string Render(Invoice invoice)
    {
        var lines = invoice.Lines.Select(l => $"  {l.Quantity} x {l.Description} @ {l.UnitPrice:0.00}");
        return $"""
            INVOICE
            {string.Join(Environment.NewLine, lines)}
              Subtotal: {invoice.Subtotal:0.00}
              VAT: {invoice.Vat:0.00}
              Total: {invoice.Total:0.00}
            """;
    }
}

/// <summary>Owned by ops: where invoices are stored.</summary>
public class InvoiceArchive
{
    public string Save(string renderedInvoice)
    {
        var path = Path.Combine(Path.GetTempPath(), $"invoice-{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, renderedInvoice);
        return path;
    }
}

/// <summary>Owned by IT: how mail is delivered.</summary>
public class InvoiceMailer
{
    public void Send(string to, string renderedInvoice) =>
        Console.WriteLine($"[Mail] Sending invoice to {to}:\n{renderedInvoice}");
}

/// <summary>
/// Orchestrates the workflow. Its single reason to change is
/// "the steps of processing an invoice change" - not any of the details.
/// </summary>
public class InvoiceProcessor(
    InvoiceCalculator calculator,
    InvoiceRenderer renderer,
    InvoiceArchive archive,
    InvoiceMailer mailer)
{
    public void Process(string customerEmail, IReadOnlyList<InvoiceLine> lines)
    {
        var invoice = calculator.Calculate(lines);
        var rendered = renderer.Render(invoice);
        archive.Save(rendered);
        mailer.Send(customerEmail, rendered);
    }
}
