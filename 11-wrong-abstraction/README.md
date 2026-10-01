# 11 · The Wrong Abstraction (DRY gone wrong)

> *"Duplication is far cheaper than the wrong abstraction."* — Sandi Metz

DRY is about not duplicating **knowledge**, not about not duplicating **code that looks similar**.

```bash
dotnet run --project 11-wrong-abstraction/WrongAbstraction
```

## The example: one exporter, three customers

The header of [`Bad.cs`](WrongAbstraction/Bad.cs) tells the story: a CSV export for sales gets reused for accounting (add a flag), then for the bank's fixed-width file (add a flag), then header/delimiter/minor-units options (more flags).

Result: `Export(sales, includeHeader, forAccounting, bankFormat, delimiter, amountsInMinorUnits)` — 2⁴ × N combinations, most of them meaningless. The demo calls it with `forAccounting: true, bankFormat: true`. What should that produce? Nobody knows.

**Good:** three small exports that each serve one stakeholder, plus *one* genuinely shared piece of knowledge (`Csv.Field` quoting rules).

## How to recognise the wrong abstraction

- Boolean / enum parameters that select code paths (control coupling — see [03-high-coupling](../03-high-coupling)).
- `if (caller is X)`-style branches inside "shared" code.
- Every change for one caller requires regression testing all callers.
- Parameters only one caller ever passes.

## Sandi Metz's recipe

1. Re-introduce duplication: inline the abstraction back into every caller.
2. In each caller, delete the parts that don't apply.
3. Look again. *Now* extract what's truly shared — if anything.

## Talking points

- **Rule of three:** tolerate duplication until the third occurrence; by then you know what actually varies.
- Ask: *"If this rule changes, must all copies change?"* Yes → it's knowledge, deduplicate. No → it's coincidence, leave it.
- Shared libraries / "Common" projects across teams are the same problem at a larger scale.
- Vertical slices ([05](../05-ntier-vs-vertical-slice)) deliberately accept some duplication for this reason.

## Exercise

The bank changes its spec: amounts must now include a sign column (`+`/`-`). Implement it in both versions. In Bad, prove you didn't break accounting.
