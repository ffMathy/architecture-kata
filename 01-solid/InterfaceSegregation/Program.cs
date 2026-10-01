using Bad = InterfaceSegregation.Bad;
using Good = InterfaceSegregation.Good;

Console.WriteLine("=== BAD: fat interface ===");
new Bad.ReportPrinter(new Bad.CheapInkjet()).PrintQuarterlyReport();
try
{
    // Compiles fine. Explodes at runtime.
    new Bad.ArchiveScanner(new Bad.CheapInkjet()).ScanInvoices();
}
catch (NotImplementedException)
{
    Console.WriteLine("💥 CheapInkjet.Scan threw NotImplementedException at runtime");
}

Console.WriteLine();
Console.WriteLine("=== GOOD: role interfaces ===");
new Good.ReportPrinter(new Good.CheapInkjet()).PrintQuarterlyReport();
new Good.ArchiveScanner(new Good.OfficeMegaPrinter3000()).ScanInvoices();
// new Good.ArchiveScanner(new Good.CheapInkjet());
//   ^ compile error: CheapInkjet is not an IScanner. Mistake caught before running.
