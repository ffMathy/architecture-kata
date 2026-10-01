namespace OpenClosed.Bad;

public record Parcel(decimal WeightKg, string Carrier);

// ❌ Every new carrier means opening this class and editing the switch.
//    The class is never "closed": it is touched by every shipping change,
//    and every change risks breaking the carriers that already work.
public class ShippingCostCalculator
{
    public decimal Calculate(Parcel parcel)
    {
        switch (parcel.Carrier)
        {
            case "PostNord":
                return 49m + parcel.WeightKg * 5m;
            case "GLS":
                return parcel.WeightKg <= 5 ? 59m : 59m + (parcel.WeightKg - 5) * 8m;
            case "DHL Express":
                return Math.Max(149m, parcel.WeightKg * 20m);
            // TODO: marketing wants "Bring" next sprint... and "Pickup in store" after that.
            default:
                throw new NotSupportedException($"Unknown carrier '{parcel.Carrier}'");
        }
    }
}
