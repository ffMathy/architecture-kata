# 10 · Make Illegal States Unrepresentable

> *"Make illegal states unrepresentable."* — Yaron Minsky

If the type system can't express an invalid state, you never have to check for it, test for it, or debug it.

```bash
dotnet run --project 10-illegal-states-unrepresentable/IllegalStates
```

## The example: an order lifecycle

```
Pending ──Pay──▶ Paid ──Ship──▶ Shipped
   │              │
   └──Cancel──▶ Cancelled ◀──Cancel──┘
```

**Bad:** `Status` is a string; `PaidAt`, `TrackingNumber`, `CancellationReason`… are all nullable.
4 statuses × 2⁵ null-combinations = 128 representable states for 4 legal ones. The demo creates a "shipped" order with no tracking number, a typo'd status, and a cancelled order with no reason.

**Good:** a closed hierarchy of `sealed record`s. Each state carries exactly the fields it needs (non-nullable), and transitions are methods that only exist on valid source states — `Shipped` has no `Cancel()`.

## Talking points

- Every `!` (null-forgiving operator) and every `default: throw` in a status switch is a place where the type system was too weak.
- Compare: an `enum` + nullable fields is better than a string, but still allows `Shipped` with `TrackingNumber == null`.
- **Persistence:** EF Core can map hierarchies (TPH with a discriminator column) — or map a flat table into these types in the repository.
- **C# unions:** native discriminated unions are being designed for C#; until then, `abstract record` + `sealed record` nested types is the idiomatic approximation (or libraries like *OneOf*).
- Related: [08-primitive-obsession](../08-primitive-obsession) (illegal *values*), [06-encapsulation](../06-encapsulation) (illegal *transitions* via methods).

## Exercise

Add a `Returned` state that's only reachable from `Shipped`, and requires a return reason and a refund amount. What does the compiler force you to update?
