using System.Globalization;

namespace WrongAbstraction.Bad;

public record Sale(DateOnly Date, string Customer, decimal Amount, string AccountCode);

// ❌ The life story of a "DRY" abstraction:
//
//   v1  Dev A writes a CSV export for the sales team.
//   v2  Dev B needs an export for accounting: "almost the same!" -> adds `forAccounting`.
//   v3  Dev C needs the bank's fixed-format file: "it's an export too!" -> adds `bankFormat`.
//   v4  The bank wants no header, accounting wants semicolons -> `includeHeader`, `delimiter`.
//   v5  Bug: accounting amounts must be in øre (cents). Fix it... carefully... for one caller only.
//
// Every caller now pays for every other caller's requirements, and nobody dares
// touch it because "it's used in three places".
public static class Exporter
{
    public static string Export(
        IEnumerable<Sale> sales,
        bool includeHeader = true,
        bool forAccounting = false,
        bool bankFormat = false,
        string delimiter = ",",
        bool amountsInMinorUnits = false)
    {
        var lines = new List<string>();

        if (includeHeader && !bankFormat)
        {
            lines.Add(forAccounting
                ? string.Join(delimiter, "Date", "Account", "Amount")
                : string.Join(delimiter, "Date", "Customer", "Amount"));
        }

        foreach (var s in sales)
        {
            var amount = amountsInMinorUnits ? (s.Amount * 100).ToString("0", CultureInfo.InvariantCulture)
                                             : s.Amount.ToString("0.00", CultureInfo.InvariantCulture);
            if (bankFormat)
            {
                // fixed width: yyyyMMdd + 20 char name + 12 char amount
                lines.Add($"{s.Date:yyyyMMdd}{s.Customer.PadRight(20)[..20]}{amount.PadLeft(12)}");
            }
            else if (forAccounting)
            {
                lines.Add(string.Join(delimiter, s.Date.ToString("yyyy-MM-dd"), s.AccountCode, amount));
            }
            else
            {
                var customer = s.Customer.Contains(delimiter) ? $"\"{s.Customer}\"" : s.Customer;
                lines.Add(string.Join(delimiter, s.Date.ToString("yyyy-MM-dd"), customer, amount));
            }
        }

        return string.Join(Environment.NewLine, lines);
    }
}
