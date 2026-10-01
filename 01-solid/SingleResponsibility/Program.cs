Console.WriteLine("=== BAD: one class does everything ===");
new SingleResponsibility.Bad.InvoiceService().Process("jane@example.com",
[
    new("Coffee beans", 2, 80m),
    new("Grinder", 1, 450m),
]);

Console.WriteLine();
Console.WriteLine("=== GOOD: one responsibility per class ===");
var processor = new SingleResponsibility.Good.InvoiceProcessor(
    new SingleResponsibility.Good.InvoiceCalculator(),
    new SingleResponsibility.Good.InvoiceRenderer(),
    new SingleResponsibility.Good.InvoiceArchive(),
    new SingleResponsibility.Good.InvoiceMailer());

processor.Process("jane@example.com",
[
    new("Coffee beans", 2, 80m),
    new("Grinder", 1, 450m),
]);
