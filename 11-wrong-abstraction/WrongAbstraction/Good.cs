using System.Globalization;

namespace WrongAbstraction.Good;

public record Sale(DateOnly Date, string Customer, decimal Amount, string AccountCode);

// ✅ Undo the abstraction: three exports that LOOK similar but change for
//    different reasons (different stakeholders!). Each is small, obvious,
//    and can evolve without asking the others for permission.
//
//    The only thing extracted is what is genuinely the same KNOWLEDGE: CSV quoting.

internal static class Csv
{
    public static string Field(string value, char delimiter) =>
        value.Contains(delimiter) || value.Contains('"') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
}

public static class SalesTeamExport
{
    public static string ToCsv(IEnumerable<Sale> sales) =>
        string.Join(Environment.NewLine,
            sales.Select(s => $"{s.Date:yyyy-MM-dd},{Csv.Field(s.Customer, ',')},{s.Amount.ToString("0.00", CultureInfo.InvariantCulture)}")
                 .Prepend("Date,Customer,Amount"));
}

public static class AccountingExport
{
    // Accounting's rules: semicolons, account codes, amounts in øre.
    public static string ToCsv(IEnumerable<Sale> sales) =>
        string.Join(Environment.NewLine,
            sales.Select(s => $"{s.Date:yyyy-MM-dd};{s.AccountCode};{(s.Amount * 100).ToString("0", CultureInfo.InvariantCulture)}")
                 .Prepend("Date;Account;Amount"));
}

public static class BankFileExport
{
    // The bank's spec: fixed width, no header, amounts in øre.
    public static string ToFixedWidth(IEnumerable<Sale> sales) =>
        string.Join(Environment.NewLine, sales.Select(s =>
            $"{s.Date:yyyyMMdd}{s.Customer.PadRight(20)[..20]}{(s.Amount * 100).ToString("0", CultureInfo.InvariantCulture).PadLeft(12)}"));
}
