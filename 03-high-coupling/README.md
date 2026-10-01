# 03 · High Coupling (and how to loosen it)

**Coupling** is the degree to which one module depends on — and must *know about* — another.
High coupling means a change in one place forces changes (or causes bugs) somewhere else.

```bash
dotnet run --project 03-high-coupling/Coupling
```

## Kinds of coupling (tightest → loosest)

Every one of these is labelled with a `// COUPLING (...)` comment in [`Bad.cs`](Coupling/Bad.cs):

| Kind | Meaning | In the example |
|---|---|---|
| **Content** | A module modifies another's internals | `CheckoutService` edits `Inventory.Stock` directly |
| **Common** | Modules share global mutable state | `AppState.CurrentUserEmail`, `AppState.LastOrderTotal` |
| **Control** | A caller passes flags that choose the callee's logic | `Send(..., isHtml, isUrgent, isMarketing)` |
| **Stamp** | A whole structure is passed when only a part is needed | (discussion point: passing `Customer` to get an email) |
| **Data** | Only the needed values are passed | `LoyaltyProgram.AwardPoints(OrderPlaced e)` in Good |
| **Message / event** | Modules only exchange messages | `EventBus.Publish(new OrderPlaced(...))` |

Plus the most common .NET smell: **`new`-ing concrete collaborators** (and `static` helpers) inside business code.

## What changes in Good

1. `CheckoutService` depends on **three** things (`IInventory`, `IPayments`, `EventBus`) instead of four concrete classes plus a global.
2. Side effects (email, loyalty, SMS) **subscribe** to `OrderPlaced`. Adding the SMS requirement touched zero lines of checkout.
3. `LoyaltyProgram` receives its data instead of reading a global — the hidden "call `LastOrderTotal = ...` first" ordering rule disappears.
4. Boolean flags were replaced by intention-revealing methods.

## Talking points

- **Coupling isn't evil; *unnecessary* coupling is.** Something has to know about everything — that's the composition root (`Program.cs`).
- **Afferent vs. efferent coupling:** how many things depend on me (Ca) vs. how many I depend on (Ce). `AppState` in Bad has huge Ca — everyone is afraid to touch it.
- **Events trade compile-time coupling for runtime indirection.** Harder to follow "what happens on checkout?" Discuss when that's worth it.
- **Temporal coupling** (things must be called in a specific order) is the sneakiest kind — the compiler won't help.
- Coupling and cohesion are two sides of one coin: see [04-low-cohesion](../04-low-cohesion).

## Exercise

Requirement: *"Orders over 1000 kr should also post to the #big-sales Slack channel."*
Implement it in both versions. Which files did you open? Which tests would need to change?
