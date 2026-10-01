# 12 · Functional Core, Imperative Shell (designing for testability)

> Put the decisions in a pure **core** (values in → values out). Put the I/O in a thin **shell** that has no decisions.
> — Gary Bernhardt, *"Boundaries"*

```bash
dotnet run  --project 12-functional-core-imperative-shell/LateFees
dotnet test 12-functional-core-imperative-shell/LateFees.Tests
```

## The example: library late fees

Rules: no action before the due date · 1–6 days late → reminder e-mail · 7+ days → charge 5 kr/day, max 200 kr · never charge twice.

**Bad:** `LateFeeJob.Run()` reads a static "database", calls `DateTime.Today`, computes fees, sends e-mails and mutates loans — all in one loop. Testing the 200 kr cap means controlling the database, the clock *and* capturing e-mails. In practice: no tests.

**Good:**
- **Core:** `LateFeePolicy.Decide(loans, today)` returns a list of `Decision`s (`SendReminder`, `ChargeFee`). Pure, deterministic.
- **Shell:** `LateFeeJob` loads loans, reads the clock (`TimeProvider`), calls the core, then executes each decision. No `if` about business rules.
- **Tests:** [`LateFeePolicyTests`](LateFees.Tests/LateFeePolicyTests.cs) — 10 tests, zero mocks, run in milliseconds.

## Talking points

- **Mocks are a design smell signal.** If a test needs 5 mocks, the code mixes decisions with side effects.
- Decisions as **data** (`ChargeFee` record) are also great for logging, dry-runs ("preview what tonight's job would do" — the demo does this), and auditing.
- `TimeProvider` (built into .NET 8+) replaces `DateTime.Now` in the shell. The core doesn't even need that — it just takes `today` as a parameter.
- Same shape at every scale: **hexagonal architecture / ports & adapters** is "functional core, imperative shell" with interfaces at the boundary.
- Relates to [DIP](../01-solid/DependencyInversion) (shell depends on abstractions) and [CQS](../13-command-query-separation) (core = queries, shell = commands).

## Exercise

New rule: *members with a "student" flag pay half.* Add it test-first to the Good version. Then try to write the same test against `Bad.LateFeeJob`.
