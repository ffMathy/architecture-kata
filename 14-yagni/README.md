# 14 · YAGNI — You Aren't Gonna Need It (speculative generality)

> *"Always implement things when you actually need them, never when you just foresee that you need them."* — Ron Jeffries

```bash
dotnet run --project 14-yagni/Yagni
```

## The example

**Requirement:** *"When a user signs up, send them a welcome e-mail."*

**Bad:** 8 interfaces and 8 classes: message templates, channel factories, a *multi-tenant* factory provider (there is one tenant), a recipient resolver, and a pipeline with a single no-op step "for later". Same output.

**Good:** one class, three lines.

Count it live in the workshop: lines of code, files, and *time to find where the e-mail text is*.

## Talking points

- The cost of speculative generality is paid **now** (build, read, test, debug) and **every day after** — for a benefit that usually never arrives.
- When the future *does* arrive, it's rarely the shape you predicted. The wrong abstraction is then in the way (see [11-wrong-abstraction](../11-wrong-abstraction)).
- **YAGNI ≠ no design.** Keep code *easy to change* (small, cohesive, well-named, tested) — that's what makes adding the abstraction later cheap.
- Tension with [Open/Closed](../01-solid/OpenClosed): don't add extension points until a real second variation appears.
- Things YAGNI does **not** cover: security, data integrity, observability basics, and decisions that are expensive to reverse (public API shapes, database schemas). Think ahead there.
- Smells: interfaces with one implementation *and* no test double, `I*Factory*Provider`, generic type parameters only ever closed over one type, config options nobody sets.

## Exercise

Product now asks for an SMS welcome message. Implement it starting from Good. Does the abstraction you end up with look like the Bad version's?
