namespace LegacyRenewalApp;

public interface ISubscriptionPlan
{
    SubscriptionPlan GetByCode(string code);
}