# 09 · Composition over Inheritance

> *"Favor object composition over class inheritance."* — Gang of Four, 1994

```bash
dotnet run --project 09-composition-over-inheritance/CompositionOverInheritance
```

## Part 1 — class explosion

**Bad:** `EmailNotifier`, `SmsNotifier`, `LoggingEmailNotifier`, `LoggingSmsNotifier`, `RetryingLoggingSmsNotifier`, … Every *combination* of features is a new subclass, and logging is copy-pasted. "Email *and* SMS" can't even be expressed.

**Good:** five tiny classes — two channels, two decorators (`LoggingNotifier`, `RetryingNotifier`) and a `CompositeNotifier` — combined at runtime:

```csharp
new CompositeNotifier(
    new LoggingNotifier(new EmailNotifier(), "email"),
    new RetryingNotifier(new LoggingNotifier(new SmsNotifier(), "sms")));
```

This is exactly how ASP.NET Core middleware, `DelegatingHandler`s in `HttpClient`, and Polly pipelines work.

## Part 2 — the fragile base class

**Bad:** `CountingTagCollection : TagCollection` overrides `Add` and `AddRange`. Because the base's `AddRange` *happens to* call `Add`, every item is counted twice. The subclass silently depends on an implementation detail of its parent — the base class can't change safely, and the subclass can't be understood alone.
(Classic example from *Effective Java*, Item 18.)

**Good:** wrap a `TagCollection` instead of inheriting from it. Only its public contract matters.

## Talking points

- Inheritance is the **tightest coupling** in OO: the subclass sees protected state and depends on the base's call graph.
- *"Is-a"* is a weak test (see Square/Rectangle in [LSP](../01-solid/LiskovSubstitution)). Better: *"is it substitutable, and will it stay that way?"*
- When inheritance **is** fine: shallow hierarchies, framework extension points designed for it (`ControllerBase`, `BackgroundService`), sealed type hierarchies modelling alternatives (see [10-illegal-states-unrepresentable](../10-illegal-states-unrepresentable)).
- C#: make classes `sealed` by default; only open them deliberately.

## Exercise

Add a "quiet hours" feature: don't send SMS between 22:00 and 07:00. How many classes in Bad? In Good?
