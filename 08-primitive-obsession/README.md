# 08 · Primitive Obsession

Using `string`, `int`, `decimal`, `Guid` for domain concepts that have rules and meaning — email addresses, money, IDs, quantities.

```bash
dotnet run --project 08-primitive-obsession/PrimitiveObsession
```

## What goes wrong (Bad)

- `PlaceOrder(int customerId, int productId, int quantity, ...)` — swap two arguments and it compiles. The demo does exactly that.
- `Total(100 DKK + 100 EUR)` returns `200`. Of what?
- Validation for "is this a valid email" is copied into every method that takes a `string email`, or forgotten.

## What changes (Good)

- `CustomerId`, `ProductId` are `readonly record struct`s — zero allocation, but **distinct types**.
- `Money` won't add different currencies. `Email` normalises and validates in its constructor.
- *Parse, don't validate:* once you hold an `Email`, it's valid. Methods downstream don't re-check.

## Talking points

- `record` / `readonly record struct` make value objects nearly free in modern C#: equality, `with`, `ToString` for free.
- **Where to convert:** at the edges (API model binding, DB mapping). EF Core value converters / complex types and `System.Text.Json` converters handle this.
- Libraries like *Vogen* or *StronglyTypedId* generate the boilerplate if you have many IDs.
- **Not everything needs a type.** A `string` used once for a log message is fine. Look for primitives that carry rules, travel far, or get mixed up.
- Value objects improve [cohesion](../04-low-cohesion) — they give behaviour like "mask this card number" an obvious home.

## Exercise

Find a method in your codebase with 3+ parameters of the same primitive type. What would break if two were swapped? Introduce a value object for one of them.
