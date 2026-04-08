namespace LegacyRenewalApp;

public interface IBillingService
{
   void SaveInvoice(RenewalInvoice invoice);
   void SendNotification(string email, string subject, string body);
}