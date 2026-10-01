# 05 · N-tier vs. Vertical Slice Architecture

Two Web APIs with **identical behaviour** (a tiny library: list/get books, borrow/return a book, max 3 loans per member, 5 kr/day late fee).
Only the *organisation of the code* differs.

```bash
dotnet run --project 05-ntier-vs-vertical-slice/Library.NTier          # http://localhost:5101
dotnet run --project 05-ntier-vs-vertical-slice/Library.VerticalSlice  # http://localhost:5102
```

Each project has a `requests.http` file you can run from VS / Rider / VS Code (REST Client).

## Side by side

```
Library.NTier/                          Library.VerticalSlice/
├── Controllers/                        ├── Features/
│   ├── BooksController.cs              │   ├── Books/
│   └── LoansController.cs              │   │   ├── ListBooks.cs     ← route + query + response
├── Services/                           │   │   └── GetBook.cs
│   ├── IBookService.cs                 │   └── Loans/
│   ├── BookService.cs                  │       ├── BorrowBook.cs    ← route + rules + response
│   ├── ILoanService.cs                 │       └── ReturnBook.cs
│   └── LoanService.cs                  ├── Domain/
├── Repositories/                       │   ├── Book.cs
│   ├── IBookRepository.cs              │   └── Loan.cs              ← shared rules (late fee)
│   ├── InMemoryBookRepository.cs       ├── Infrastructure/
│   ├── ILoanRepository.cs              │   └── LibraryDb.cs         ← "DbContext"
│   └── InMemoryLoanRepository.cs       └── Program.cs
├── Models/  (Book, Loan)
├── Dtos/    (BookDto, LoanDto, BorrowBookRequest)
├── Mapping/ (MappingExtensions)
└── Program.cs
```

**N-tier (layered):** code is grouped by *technical role*. A request flows Controller → Service → Repository → and back. Each layer may only call the one below.

**Vertical slice:** code is grouped by *feature / use case*. Each slice owns its request, response, handler and endpoint, from HTTP down to data access. Coupling *between* slices is minimised; coupling *within* a slice is maximised.

## Live demo: "add a feature"

> **New requirement:** members can *reserve* a book that's currently on loan. `POST /books/{id}/reservations`.

Count the files you have to open/create:

| | N-tier | Vertical slice |
|---|---|---|
| Endpoint | `BooksController` (edit) or new controller | `Features/Reservations/ReserveBook.cs` (new) |
| Business logic | `IReservationService` + `ReservationService` (new) — or bolt onto `LoanService` | same file |
| Data access | `IReservationRepository` + impl (new) | `LibraryDb.Reservations` (one line) |
| DTOs / mapping | `Dtos/…`, `Mapping/…` | same file |
| DI registration | `Program.cs` (2 lines) | `Program.cs` (1 line) |
| **Total** | **~7–8 files across 5 folders** | **~3 files** |

Follow-up: *"show 'reserved' in the book list"*. In N-tier you change `BookDto` — which is also returned by `GET /books/{id}` and maybe by others. In the slice version only `ListBooks.Response` changes.

## Talking points

**Why N-tier is popular**
- Easy to explain, enforces a *direction* of dependencies, consistent everywhere.
- Works well when most of the app is uniform CRUD.

**Where N-tier hurts**
- **Low cohesion by feature** (see [04-low-cohesion](../04-low-cohesion)): one feature is smeared across 5+ folders. Things that change together don't live together.
- **Pass-through layers:** look at `BookService` — it adds nothing but indirection.
- **Shared abstractions grow forever:** `ILoanRepository` gets a method for every query any service needs. `BookDto` serves every endpoint, so it's a compromise for all of them.
- **"Service" god classes:** `LoanService` will collect every loan-related use case.
- Merge conflicts: every feature edits the same few files.

**Where vertical slices hurt**
- Duplication between slices (look: "is the book on loan?" appears in three slices). That's *deliberate* — extract only when the duplication is real knowledge, not coincidence (see [11-wrong-abstraction](../11-wrong-abstraction)).
- Needs discipline: shared rules still belong in the domain (`Loan.Return` computes the late fee) — slices are not an excuse for transaction scripts everywhere.
- Less obvious "where does X go?" for newcomers without conventions.
- Popular libraries: MediatR / Wolverine / FastEndpoints / Carter — but as this project shows, plain minimal APIs are enough.

**It's not either/or.** Many teams do *vertical slices on the outside, a rich domain model in the middle*. Clean Architecture / hexagonal layers can live *inside* a slice-organised project.

## Discussion questions

1. Which version would you rather onboard a new developer into? Why?
2. How do you share *cross-cutting* concerns (auth, validation, logging) in each style? (Hint: middleware, endpoint filters, pipeline behaviours.)
3. How would you unit test `BorrowBook.Handle`? How would you unit test `LoanService.Borrow`? Which needs more mocks?
