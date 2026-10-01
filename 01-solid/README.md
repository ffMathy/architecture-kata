# 01 · SOLID

Five principles (Robert C. Martin) for keeping object-oriented code easy to change.
Each principle has its own runnable project with a `Bad.cs`, a `Good.cs` and a `Program.cs` that runs both.

| Project | Principle | One-liner |
|---|---|---|
| [`SingleResponsibility`](SingleResponsibility) | **S**ingle Responsibility | A class should have one *reason to change* (one actor it serves). |
| [`OpenClosed`](OpenClosed) | **O**pen/Closed | Add behaviour by adding code, not by editing working code. |
| [`LiskovSubstitution`](LiskovSubstitution) | **L**iskov Substitution | A subtype must keep every promise its base type makes. |
| [`InterfaceSegregation`](InterfaceSegregation) | **I**nterface Segregation | Clients shouldn't depend on methods they don't use. |
| [`DependencyInversion`](DependencyInversion) | **D**ependency Inversion | Policy shouldn't depend on details; both depend on abstractions owned by the policy. |

```bash
dotnet run --project 01-solid/SingleResponsibility
dotnet run --project 01-solid/OpenClosed
dotnet run --project 01-solid/LiskovSubstitution
dotnet run --project 01-solid/InterfaceSegregation
dotnet run --project 01-solid/DependencyInversion
```

---

## S — Single Responsibility

**Bad:** `InvoiceService.Process` calculates VAT, renders text, writes a file and "sends" an e-mail.

**Talking points**
- "Responsibility" ≠ "does one thing". It means *one reason to change* — one stakeholder.
  Ask the room: *who* would request a change to each block of code? (Accounting, marketing, ops, IT.)
- Merge conflicts and regressions happen when four teams edit one file.
- The `Good` version still has an orchestrator (`InvoiceProcessor`). SRP doesn't forbid coordination — coordination is a responsibility too.

**Exercise:** Add a PDF output and a 12 % reduced VAT rate for books. Which files change in Bad vs Good?

## O — Open/Closed

**Bad:** a `switch` over carrier names. Every new carrier edits the same method.

**Talking points**
- "Closed" means *the existing, tested code doesn't need to be reopened*.
- The extension point is `IShippingMethod`. In ASP.NET you'd register implementations in DI and inject `IEnumerable<IShippingMethod>`.
- Don't pre-build extension points everywhere (see [14-yagni](../14-yagni)). Introduce them when the *second or third* variation shows up.

**Exercise:** Add "Bring" with a price of 39 kr + 6 kr/kg. Count edited lines in each version.

## L — Liskov Substitution

**Bad:** `Square : Rectangle` and `ReadOnlyArchiveStore : FileStore` both compile, both break callers at runtime.

**Talking points**
- LSP is about *behavioural* contracts, not just signatures: preconditions can't be strengthened, postconditions can't be weakened, invariants must hold.
- Red flags: `throw new NotSupportedException()` in an override, or `if (x is SomeSubtype)` in calling code.
- .NET's own `ICollection<T>.IsReadOnly` + `ReadOnlyCollection<T>.Add` throwing is a real-world LSP smell worth showing.

**Exercise:** Write a "test" for `Rectangle` that passes for Rectangle but fails for Square. That test *is* the contract.

## I — Interface Segregation

**Bad:** `IMultiFunctionDevice` forces `CheapInkjet` to throw `NotImplementedException` for three of four methods.

**Talking points**
- Fat interfaces create invisible coupling: `ReportPrinter` depends on `Fax` even though it never faxes.
- Mocks/test doubles get painful when interfaces are wide — a great practical signal.
- Interfaces belong to the *client* (role interfaces), not to the implementation ("header interfaces" that mirror every public method of a class).

**Exercise:** Find an `I<ClassName>` in your own codebase that is a copy of the class' public surface. How many consumers use all of it?

## D — Dependency Inversion

**Bad:** `OrderService` does `new SqlServerOrderRepository()` and `new SmtpMailer()`.

**Talking points**
- DIP (a design principle) ≠ DI (a technique) ≠ IoC container (a tool). You can do DIP with plain constructors, as `Program.cs` shows.
- *Inversion* = the abstraction is owned by the high-level module (`IOrderRepository` lives next to `OrderService`, not in the SQL project). Source-code dependencies point *toward* the policy.
- Testability is a symptom of good design, not the goal.

**Exercise:** Add a "send a Slack message to sales for orders > 10.000 kr" requirement. Where does the code go?
