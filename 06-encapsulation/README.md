# 06 · Encapsulation

**Encapsulation** = bundling data with the behaviour that operates on it, and *hiding* the data so it can only change in valid ways.

It's not "make fields private and generate a getter and setter for each". A public setter is a public field with extra steps.

```bash
dotnet run --project 06-encapsulation/Encapsulation
```

## The example: a bank account

Invariants the business cares about:
1. Balance equals the sum of transactions.
2. Balance never goes below the overdraft limit.
3. A frozen account cannot be debited.

| | Bad (anemic model) | Good (rich model) |
|---|---|---|
| Who enforces the rules? | Every service, hopefully | The `BankAccount` itself |
| `Balance` | Settable field — can drift from transactions | Derived from transactions |
| `Transactions` | Public `List<T>` — anybody can `Clear()` | `IReadOnlyList<T>` via `AsReadOnly()` |
| New payment channel | Must re-implement every rule | Calls `Withdraw` — can't forget a rule |

The demo shows `MobilePayService` (written "by another team") withdrawing from a frozen account in the Bad version.

## Talking points

- **Anemic Domain Model** (Fowler) — objects with no behaviour, plus "services" that hold all the logic. Sometimes fine (CRUD, DTOs), dangerous when there are real business rules.
- **Constructors are part of encapsulation.** An object should be valid from the moment it exists — no `new Account(); account.Init(...)`.
- **Leaky collections:** returning `List<T>` exposes mutation. `IReadOnlyList<T>` alone can be cast back — `AsReadOnly()` can't.
- **C# tools:** `private set`, `init`, `required`, records, `IReadOnly*`, and in C# 14 the `field` keyword (see `OverdraftLimit`) for validated auto-properties.
- **What about EF Core?** EF Core maps private fields and backing fields fine (`HasField`, `UsePropertyAccessMode`) — persistence isn't a reason to expose setters.
- Related: [02-law-of-demeter](../02-law-of-demeter), [07-tell-dont-ask](../07-tell-dont-ask), [10-illegal-states-unrepresentable](../10-illegal-states-unrepresentable).

## Exercise

New rule: *"Withdrawals over 10.000 kr in a single day require the account to be verified."*
Add it to both versions. In Bad, how many places did you have to find?
