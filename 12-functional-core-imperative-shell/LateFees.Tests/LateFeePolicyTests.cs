using LateFees.Good;

namespace LateFees.Tests;

// No mocks. No database. No fake clock. Just values in, values out.
// Compare with LateFees.Bad.LateFeeJob - try writing any of these tests for it.
public class LateFeePolicyTests
{
    private static readonly DateOnly Today = new(2026, 10, 1);

    private static Loan LoanDue(int daysAgo, bool feeCharged = false) =>
        new(Id: 1, MemberEmail: "jane@example.com", DueOn: Today.AddDays(-daysAgo), FeeCharged: feeCharged);

    [Fact]
    public void Loan_not_yet_due_needs_no_action() =>
        Assert.Null(LateFeePolicy.Decide(LoanDue(daysAgo: -5), Today));

    [Fact]
    public void Loan_due_today_is_not_late() =>
        Assert.Null(LateFeePolicy.Decide(LoanDue(daysAgo: 0), Today));

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    public void Within_grace_period_member_gets_a_reminder(int daysLate) =>
        Assert.Equal(new SendReminder(1, "jane@example.com", daysLate), LateFeePolicy.Decide(LoanDue(daysLate), Today));

    [Theory]
    [InlineData(7, 35)]
    [InlineData(20, 100)]
    [InlineData(40, 200)]
    [InlineData(400, 200)]
    public void After_grace_period_fee_is_5_per_day_capped_at_200(int daysLate, decimal expectedFee) =>
        Assert.Equal(new ChargeFee(1, "jane@example.com", expectedFee), LateFeePolicy.Decide(LoanDue(daysLate), Today));

    [Fact]
    public void Fee_is_never_charged_twice() =>
        Assert.Null(LateFeePolicy.Decide(LoanDue(daysAgo: 20, feeCharged: true), Today));

    [Fact]
    public void Decides_for_many_loans_at_once()
    {
        Loan[] loans = [LoanDue(-1), LoanDue(3) with { Id = 2 }, LoanDue(10) with { Id = 3 }];

        var decisions = LateFeePolicy.Decide(loans, Today);

        Assert.Collection(decisions,
            d => Assert.IsType<SendReminder>(d),
            d => Assert.IsType<ChargeFee>(d));
    }
}
