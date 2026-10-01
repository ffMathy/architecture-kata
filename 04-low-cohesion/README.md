# 04 · Low Cohesion

**Cohesion** (sometimes called *coherence*) is how strongly the things inside a module belong together.
High cohesion: a class/module is about one concept, and its members use each other's data.
Low cohesion: a module is a bag of unrelated things.

> **Low coupling + high cohesion** is the goal. They pull in the same direction: when related things live together, fewer connections cross module boundaries.

```bash
dotnet run --project 04-low-cohesion/Cohesion
```

## Part 1 — the junk drawer (`Helpers`)

`Helpers` contains e-mail validation, VAT, date formatting, slugs, age calculation and card masking.
This is **coincidental cohesion** — the weakest kind.

In `Good`, each piece of behaviour moved next to its concept: `EmailAddress`, `Price`, `CardNumber`.
(Notice how this overlaps with [08-primitive-obsession](../08-primitive-obsession).)

## Part 2 — the god object (`UserAccount`)

Draw the field/method matrix on a whiteboard:

| Method | Username | PasswordHash | FailedLogins | CardNumber | Balance | Invoices | Language | DarkMode |
|---|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
| `Login` | | ✔ | ✔ | | | | | |
| `ChangePassword` | | ✔ | | | | | | |
| `Charge` | | | | | ✔ | ✔ | | |
| `BillingSummary` | | | | ✔ | ✔ | ✔ | | |
| `ToggleDarkMode` | | | | | | | | ✔ |
| `Greeting` | | | | | | | ✔ | |

Three disconnected clusters = three classes waiting to get out. That's the intuition behind **LCOM** (Lack of Cohesion of Methods) metrics.

## Levels of cohesion (worst → best)

Coincidental → Logical (`if type == ...`) → Temporal (things run "at startup") → Procedural → Communicational (same data) → Sequential (output of one feeds the next) → **Functional** (everything contributes to one well-defined task).

## Talking points

- `*Manager`, `*Helper`, `*Utils`, `*Service` with 40 methods — names that say nothing usually hold nothing in common.
- Cohesion applies at **every level**: method, class, namespace, project, service. A microservice with low cohesion is a distributed junk drawer.
- **Package-level cohesion** is the key argument in [05-ntier-vs-vertical-slice](../05-ntier-vs-vertical-slice): N-tier groups by *technical* kind (all controllers together), vertical slices group by *feature* (everything about "borrow a book" together). Which has higher cohesion?
- Common Closure Principle: *gather together what changes for the same reason* — SRP at package level.

## Exercise

Find the biggest class in your codebase. Build the field/method matrix. How many clusters do you see?
