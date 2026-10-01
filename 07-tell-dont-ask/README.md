# 07 · Tell, Don't Ask

> *"Rather than asking an object for data and acting on that data, we should instead tell an object what to do."* — Andy Hunt & Dave Thomas

```bash
dotnet run --project 07-tell-dont-ask/TellDontAsk
```

## The example: subscription renewal

**Bad:** `RenewalJob` and `AdminController` both read five properties of `Subscription`, decide whether to renew, and then set `ExpiresOn`.
The admin copy forgot two conditions — and renews a *cancelled free* plan. The demo shows it.

**Good:** `subscription.RenewIfDue(today)`. The rule exists exactly once, next to the data it needs.

## Talking points

- **Smell:** a chain of `if (x.A && x.B && !x.C)` followed by `x.D = ...`. Logic that uses *only* another object's data belongs *in* that object (Fowler: *Feature Envy*).
- Tell-Don't-Ask is the *behavioural* side of [encapsulation](../06-encapsulation), and the cure for most [Law of Demeter](../02-law-of-demeter) violations.
- **Queries are still fine.** UIs and reports need to *ask* (`ExpiresOn` is still public). The principle is about not making *decisions* on someone else's behalf. See also [13-command-query-separation](../13-command-query-separation).
- **Don't overdo it:** putting rendering, persistence and e-mail into `Subscription` "because it has the data" violates SRP. Tell it to make *its own* decisions; let other objects handle other concerns.

## Exercise

Add a rule: *"Enterprise subscriptions renew yearly, not monthly."* Where does it go in each version? How many call sites must you check in Bad?
