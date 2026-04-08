namespace LegacyRenewalApp;

public interface IDiscountCalculator
{
    (decimal Amount, string Notes) Calculate(Customer customer, decimal baseAmount, int seatCount, bool useLoyaltyPoints);
}