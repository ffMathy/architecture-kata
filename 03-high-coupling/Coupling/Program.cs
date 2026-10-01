using Bad = Coupling.Bad;
using Good = Coupling.Good;

Console.WriteLine("=== BAD: checkout is welded to everything ===");
new Bad.CheckoutService().Checkout("jane@example.com", "coffee", 2, 80m);

// Hidden coupling in action: someone else touches the global state between calls...
Bad.AppState.CurrentUserEmail = "someone.else@example.com";
new Bad.PaymentGateway().Charge(10);
Console.WriteLine($"[Log] {string.Join(" | ", Bad.AppState.Log)}  <- who set that user? 🤔");

Console.WriteLine();
Console.WriteLine("=== GOOD: checkout publishes an event; subscribers react ===");
var bus = new Good.EventBus();
var email = new Good.EmailService();
var loyalty = new Good.LoyaltyProgram();
var sms = new Good.SmsService();

bus.Subscribe(email.SendReceipt);
bus.Subscribe(loyalty.AwardPoints);
bus.Subscribe(sms.NotifyWarehouse); // new requirement = new subscriber; CheckoutService untouched

new Good.CheckoutService(new Good.InMemoryInventory(), new Good.FakePayments(), bus)
    .Checkout("jane@example.com", "coffee", 2, 80m);
