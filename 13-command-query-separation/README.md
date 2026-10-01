# 13 · Command-Query Separation (CQS)

> *"Asking a question should not change the answer."* — Bertrand Meyer

Every method should be **either**
- a **query**: returns data, has no observable side effects, or
- a **command**: changes state, returns nothing (pragmatically: an id or success flag of what it just did).

```bash
dotnet run --project 13-command-query-separation/CommandQuerySeparation
```

## The examples

| Bad | Problem | Good |
|---|---|---|
| `GetNextInvoiceNumber()` increments | A debug log line burns an invoice number | `PeekNextInvoiceNumber()` + `ReserveInvoiceNumber()` |
| `GetTotal()` consumes a one-time discount | Showing the cart changes what checkout charges | `ApplyWelcomeDiscount()` + `Total` |
| `IsValid(password)` counts failed attempts | Calling it twice for UI + audit locks the user out | `Matches()`, `IsLockedOut`, `TryLogin()` |

## Talking points

- Queries you can call freely are a superpower: logging, debugging, caching, retries, property-based tests.
- **Naming is the contract.** `Get*`, `Is*`, `Has*`, properties ⇒ no side effects. A side effect needs a verb: `Reserve`, `Apply`, `Try*`.
- Pragmatic exceptions: `Stack.Pop()`, `Interlocked.Increment`, `Queue.Dequeue`, `Dictionary.TryAdd` — atomicity sometimes wins. Name them so it's obvious.
- **CQRS** is CQS scaled up to architecture: separate *models* (and sometimes stores) for reads and writes. The vertical slices in [05](../05-ntier-vs-vertical-slice) are a lightweight version: `ListBooks` (query) and `BorrowBook` (command) share nothing but the database.

## Exercise

Search your codebase for `Get` methods that call `SaveChanges`, `Add`, `Remove`, `++` or send something. Rename or split one.
