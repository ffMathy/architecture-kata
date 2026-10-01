using Bad = WrongAbstraction.Bad;
using Good = WrongAbstraction.Good;

Bad.Sale[] badSales = [new(new(2026, 10, 1), "Hansen, Jens", 1299.50m, "1010"), new(new(2026, 10, 2), "Acme ApS", 89m, "1020")];
Good.Sale[] goodSales = [new(new(2026, 10, 1), "Hansen, Jens", 1299.50m, "1010"), new(new(2026, 10, 2), "Acme ApS", 89m, "1020")];

Console.WriteLine("=== BAD: one export to rule them all ===");
Console.WriteLine("-- sales team:");
Console.WriteLine(Bad.Exporter.Export(badSales));
Console.WriteLine("-- accounting:");
Console.WriteLine(Bad.Exporter.Export(badSales, forAccounting: true, delimiter: ";", amountsInMinorUnits: true));
Console.WriteLine("-- bank:");
Console.WriteLine(Bad.Exporter.Export(badSales, includeHeader: false, bankFormat: true, amountsInMinorUnits: true));
Console.WriteLine("-- someone tries a 'reasonable' combination:");
Console.WriteLine(Bad.Exporter.Export(badSales, forAccounting: true, bankFormat: true)); // which wins? header? øre? 🤷

Console.WriteLine();
Console.WriteLine("=== GOOD: three honest exports + one shared helper ===");
Console.WriteLine("-- sales team:");
Console.WriteLine(Good.SalesTeamExport.ToCsv(goodSales));
Console.WriteLine("-- accounting:");
Console.WriteLine(Good.AccountingExport.ToCsv(goodSales));
Console.WriteLine("-- bank:");
Console.WriteLine(Good.BankFileExport.ToFixedWidth(goodSales));
