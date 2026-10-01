namespace OpenClosed.Good;

public record Parcel(decimal WeightKg);

// ✅ Open for extension: add a new IShippingMethod.
//    Closed for modification: ShippingCostCalculator never changes.
public interface IShippingMethod
{
    string Name { get; }
    decimal PriceFor(Parcel parcel);
}

public class PostNord : IShippingMethod
{
    public string Name => "PostNord";
    public decimal PriceFor(Parcel parcel) => 49m + parcel.WeightKg * 5m;
}

public class Gls : IShippingMethod
{
    public string Name => "GLS";
    public decimal PriceFor(Parcel parcel) =>
        parcel.WeightKg <= 5 ? 59m : 59m + (parcel.WeightKg - 5) * 8m;
}

public class DhlExpress : IShippingMethod
{
    public string Name => "DHL Express";
    public decimal PriceFor(Parcel parcel) => Math.Max(149m, parcel.WeightKg * 20m);
}

// 👇 The new requirement: a new file, zero edits to existing code.
public class PickupInStore : IShippingMethod
{
    public string Name => "Pickup in store";
    public decimal PriceFor(Parcel parcel) => 0m;
}

public class ShippingCostCalculator(IEnumerable<IShippingMethod> methods)
{
    private readonly Dictionary<string, IShippingMethod> _methods =
        methods.ToDictionary(m => m.Name);

    public IEnumerable<string> AvailableCarriers => _methods.Keys;

    public decimal Calculate(Parcel parcel, string carrier) =>
        _methods.TryGetValue(carrier, out var method)
            ? method.PriceFor(parcel)
            : throw new NotSupportedException($"Unknown carrier '{carrier}'");
}
