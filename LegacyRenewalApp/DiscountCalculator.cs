namespace LegacyRenewalApp;

public class DiscountCalculator : IDiscountCalculator
{
    public (decimal Amount, string Notes) Calculate(Customer customer, decimal baseAmount, int seatCount, bool useLoyaltyPoints)
    {
        decimal discountAmount = 0m;
        string notes = string.Empty;
        
        if (customer.Segment == "Silver") { discountAmount += baseAmount * 0.05m; notes += "silver discount; "; }
        else if (customer.Segment == "Gold") { discountAmount += baseAmount * 0.10m; notes += "gold discount; "; }
        else if (customer.Segment == "Platinum") { discountAmount += baseAmount * 0.15m; notes += "platinum discount; "; }
        
        if (customer.YearsWithCompany >= 5) { discountAmount += baseAmount * 0.07m; notes += "long-term discount; "; }
        else if (customer.YearsWithCompany >= 2) { discountAmount += baseAmount * 0.03m; notes += "basic discount; "; }
        
        if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
        {
            int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            discountAmount += pointsToUse;
            notes += $"loyalty points used: {pointsToUse}; ";
        }

        return (discountAmount, notes.Trim());
    }
}