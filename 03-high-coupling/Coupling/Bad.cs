namespace Coupling.Bad;

// ❌ HIGH COUPLING. Every comment tagged COUPLING names the kind of coupling
//    (from tightest to loosest: content > common > control > stamp > data).

// COUPLING (common): global mutable state that "everybody" reads and writes.
public static class AppState
{
    public static string CurrentUserEmail = "";
    public static decimal LastOrderTotal;
    public static List<string> Log = [];
}

public class Inventory
{
    // COUPLING (content): other classes reach straight into this dictionary.
    public Dictionary<string, int> Stock = new() { ["coffee"] = 10, ["grinder"] = 1 };
}

public class PaymentGateway
{
    public bool Charge(decimal amount)
    {
        // COUPLING (common): payment quietly depends on whoever set the "current user".
        AppState.Log.Add($"charged {amount} to {AppState.CurrentUserEmail}");
        return true;
    }
}

public class EmailService
{
    // COUPLING (control): a flag tells the callee WHICH algorithm to run.
    public void Send(string to, string body, bool isHtml, bool isUrgent, bool isMarketing)
    {
        var kind = isMarketing ? "marketing" : isUrgent ? "urgent" : "normal";
        Console.WriteLine($"[Email/{kind}/{(isHtml ? "html" : "text")}] {to}: {body}");
    }
}

public class LoyaltyProgram
{
    // COUPLING (common): reads a value some OTHER class must have set first.
    //                    Call order now matters and nothing enforces it.
    public void AwardPoints() =>
        Console.WriteLine($"[Loyalty] {AppState.CurrentUserEmail} earns {(int)(AppState.LastOrderTotal / 10)} points");
}

public class CheckoutService
{
    // COUPLING (concrete instantiation): checkout knows and creates every collaborator.
    private readonly Inventory _inventory = new();
    private readonly PaymentGateway _payments = new();
    private readonly EmailService _email = new();
    private readonly LoyaltyProgram _loyalty = new();

    public void Checkout(string userEmail, string product, int quantity, decimal price)
    {
        AppState.CurrentUserEmail = userEmail;

        // COUPLING (content): manipulates Inventory's internal data structure directly.
        if (!_inventory.Stock.ContainsKey(product) || _inventory.Stock[product] < quantity)
            throw new InvalidOperationException("Out of stock");
        _inventory.Stock[product] -= quantity;

        var total = quantity * price;
        _payments.Charge(total);

        AppState.LastOrderTotal = total;        // must happen BEFORE AwardPoints. Good luck remembering.
        _email.Send(userEmail, $"Thanks for buying {product}", isHtml: true, isUrgent: false, isMarketing: false);
        _loyalty.AwardPoints();

        // Product wants an SMS too, and analytics, and a webhook to the warehouse...
        // each one is another edit to THIS method.
    }
}
