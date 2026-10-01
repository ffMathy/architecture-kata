using Bad = Yagni.Bad;
using Good = Yagni.Good;

Console.WriteLine("=== BAD: 8 interfaces, 8 classes, 1 e-mail ===");
new Bad.WelcomeMessageOrchestrator<Bad.User>(
        new Bad.WelcomeTemplate(),
        new Bad.MultiTenantChannelFactoryProvider(),
        new Bad.UserEmailRecipientResolver(),
        [new Bad.NoOpPipelineStep()])
    .Orchestrate(new Bad.User("Jane", "jane@example.com"));

Console.WriteLine();
Console.WriteLine("=== GOOD: 1 class, 1 e-mail ===");
new Good.WelcomeEmail().SendTo(new Good.User("Jane", "jane@example.com"));
