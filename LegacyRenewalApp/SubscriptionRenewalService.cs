using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly IBillingService _billingService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlan _planRepository;
        private readonly IDiscountCalculator _discountCalculator;

        public SubscriptionRenewalService() : this(
            new LegacyBillingAdapter(),
            new CustomerRepository(),
            new SubscriptionPlanRepository(),
            new DiscountCalculator())
        {
            
        }
        public SubscriptionRenewalService(
            IBillingService billingService,
            ICustomerRepository customerRepository,
            ISubscriptionPlan planRepository,
            IDiscountCalculator discountCalculator)
        {
            _billingService = billingService;
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _discountCalculator = discountCalculator;
        }

        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(planCode.Trim().ToUpperInvariant());

            if (!customer.IsActive) throw new InvalidOperationException("Inactive customer");

            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
            
            var discount = _discountCalculator.Calculate(customer, baseAmount, seatCount, useLoyaltyPoints);
            
            decimal finalAmount = baseAmount - discount.Amount;
            
            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}",
                CustomerName = customer.FullName,
                PlanCode = plan.Code,
                BaseAmount = baseAmount,
                DiscountAmount = discount.Amount,
                FinalAmount = Math.Round(finalAmount, 2),
                Notes = discount.Notes,
                GeneratedAt = DateTime.UtcNow
            };
            
            _billingService.SaveInvoice(invoice);
            
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                _billingService.SendNotification(customer.Email, "Renewal", "Body of the email");
            }

            return invoice;
        }
    }
}