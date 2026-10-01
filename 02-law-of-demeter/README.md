# 02 · Law of Demeter (Principle of Least Knowledge)

> *"Only talk to your immediate friends."*

A method may call methods on: itself, its parameters, objects it creates, and its own fields.
It should **not** call methods on objects *returned* by those — i.e. avoid `a.GetB().GetC().DoSomething()`.

```bash
dotnet run --project 02-law-of-demeter/LawOfDemeter
```

## The example: the Paperboy and the Wallet

| | Bad | Good |
|---|---|---|
| Payment | `customer.Wallet.TotalMoney -= amount` | `customer.TryPay(amount)` |
| Address | `customer.Address.Street.Name` | `customer.DeliveryLabel()` |
| Knowledge the paperboy needs | Customer, Wallet, its fields, Address, Street | Customer |

Would you hand your wallet to the paperboy and let them take the money out?

## Talking points

- **Structural coupling:** `Paperboy` in the Bad version depends on the *shape* of four classes. Rename `TotalMoney` and the paperboy breaks.
- **Behaviour in the wrong place:** "do I have enough money?" is the customer's decision, not the paperboy's. LoD violations usually point at a missing method on the object you're digging into (see also [07-tell-dont-ask](../07-tell-dont-ask)).
- **Mocking pain:** testing the Bad paperboy requires building a customer *with* a wallet *with* money — deep object graphs in tests are a smell signal.
- **It's not the "one-dot rule".** Fluent builders, LINQ chains and DTOs/records that are *just data* are fine. The rule is about not reaching through an object to manipulate its collaborators.
- **Trade-off:** blindly adding wrapper methods (`customer.GetWalletTotalMoney()`) just moves the problem. Ask *what the caller is trying to achieve* and give it that.

## Exercise

1. Add a new payment option: customers can have a *tab* that they settle monthly. How many classes change in Bad vs Good?
2. Search your codebase for `\.\w+\.\w+\.\w+\.` (3+ property hops). Pick one and discuss what method is missing.
