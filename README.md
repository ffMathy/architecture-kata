# Architecture Kata

Runnable .NET 10 examples for a workshop on software design principles.
Every topic has a **Bad** and a **Good** version side by side, a `Program.cs` that runs both so the difference is visible in the console, and a `README.md` with talking points, discussion questions and a hands-on exercise.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Any editor. Open `ArchitectureKata.slnx` in Visual Studio 2022 17.13+/2026, Rider, or VS Code with C# Dev Kit.

```bash
dotnet build ArchitectureKata.slnx       # build everything
dotnet test  ArchitectureKata.slnx       # run the tests (topic 12)
dotnet run --project 02-law-of-demeter/LawOfDemeter   # run a single demo
```

## Topics

| # | Topic | Example domain | Run |
|---|---|---|---|
| 01 | [**SOLID**](01-solid) | | |
| | ↳ Single Responsibility | Invoice processing | `dotnet run --project 01-solid/SingleResponsibility` |
| | ↳ Open/Closed | Shipping carriers | `dotnet run --project 01-solid/OpenClosed` |
| | ↳ Liskov Substitution | Square/Rectangle, read-only store | `dotnet run --project 01-solid/LiskovSubstitution` |
| | ↳ Interface Segregation | Multi-function printer | `dotnet run --project 01-solid/InterfaceSegregation` |
| | ↳ Dependency Inversion | Order service + SQL/SMTP | `dotnet run --project 01-solid/DependencyInversion` |
| 02 | [**Law of Demeter**](02-law-of-demeter) | The paperboy and the wallet | `dotnet run --project 02-law-of-demeter/LawOfDemeter` |
| 03 | [**High Coupling**](03-high-coupling) | Checkout with globals vs. events | `dotnet run --project 03-high-coupling/Coupling` |
| 04 | [**Low Cohesion**](04-low-cohesion) | `Helpers` junk drawer, god object | `dotnet run --project 04-low-cohesion/Cohesion` |
| 05 | [**N-tier vs. Vertical Slice**](05-ntier-vs-vertical-slice) | Library lending Web API ×2 | `dotnet run --project 05-ntier-vs-vertical-slice/Library.NTier` |
| 06 | [**Encapsulation**](06-encapsulation) | Bank account (anemic vs. rich) | `dotnet run --project 06-encapsulation/Encapsulation` |

### Bonus topics

| # | Topic | Example domain | Run |
|---|---|---|---|
| 07 | [**Tell, Don't Ask**](07-tell-dont-ask) | Subscription renewal | `dotnet run --project 07-tell-dont-ask/TellDontAsk` |
| 08 | [**Primitive Obsession**](08-primitive-obsession) | Swapped IDs, DKK + EUR | `dotnet run --project 08-primitive-obsession/PrimitiveObsession` |
| 09 | [**Composition over Inheritance**](09-composition-over-inheritance) | Notifier class explosion, fragile base class | `dotnet run --project 09-composition-over-inheritance/CompositionOverInheritance` |
| 10 | [**Make Illegal States Unrepresentable**](10-illegal-states-unrepresentable) | Order lifecycle | `dotnet run --project 10-illegal-states-unrepresentable/IllegalStates` |
| 11 | [**The Wrong Abstraction**](11-wrong-abstraction) | One exporter with six flags | `dotnet run --project 11-wrong-abstraction/WrongAbstraction` |
| 12 | [**Functional Core, Imperative Shell**](12-functional-core-imperative-shell) | Late fees + mock-free unit tests | `dotnet test 12-functional-core-imperative-shell/LateFees.Tests` |
| 13 | [**Command-Query Separation**](13-command-query-separation) | Invoice numbers, cart discount | `dotnet run --project 13-command-query-separation/CommandQuerySeparation` |
| 14 | [**YAGNI**](14-yagni) | 16 types to send one e-mail | `dotnet run --project 14-yagni/Yagni` |

## How the topics connect

```
              Encapsulation (06) ──── Tell, Don't Ask (07) ──── Law of Demeter (02)
                     │                                                │
     Illegal states (10) ── Primitive obsession (08)                 │
                                        │                             │
                              High cohesion (04) ◀──────▶ Low coupling (03)
                                   │                              │
          Vertical slices (05) ────┘                              ├── Dependency Inversion (01)
                                                                  ├── Composition > Inheritance (09)
   Wrong abstraction (11) ── YAGNI (14) ── Open/Closed (01)       └── Functional core (12) ── CQS (13)
```

A good closing message: almost every principle here is a different angle on **"keep things that change together close, and things that change for different reasons apart."**

## Suggested workshop flow (~3 hours)

| Time | Block |
|---|---|
| 0:00 | Intro: why design matters — cost of change over time |
| 0:15 | **01 SOLID** — one principle every ~8 minutes, run each demo live |
| 1:00 | **03 Coupling** + **04 Cohesion** — the two forces behind everything else |
| 1:30 | ☕ Break |
| 1:40 | **02 Law of Demeter**, **06 Encapsulation**, **07 Tell Don't Ask** — one story, three angles |
| 2:10 | **05 N-tier vs. Vertical Slice** — live "add a reservation feature" exercise in both, in pairs |
| 2:45 | Lightning round of bonus topics (pick 2–3 the audience votes for) |
| 3:00 | Wrap-up: which smells did you recognise from *your* codebase? |

Every topic README ends with an **exercise** — good for pair work or homework.

## Repository layout

```
<topic>/
├── README.md          talking points, discussion questions, exercise
└── <Project>/
    ├── Bad.cs         the smell
    ├── Good.cs        the refactored version
    └── Program.cs     runs both, printing 💥 where Bad goes wrong and ✅ where Good holds
```

Topic 05 is the exception: two complete minimal Web APIs (`Library.NTier`, `Library.VerticalSlice`) with identical behaviour, so you can compare the structures directly.
