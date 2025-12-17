namespace Mockups.Models.Analytics
{
    public class AnalyticsViewModel
    {
        public ConversionRateViewModel ConversionRateData { get; set; } = new ConversionRateViewModel();
        public LastMonthViewModel LastMonthData { get; set; } = new LastMonthViewModel();
        public CustomerBehaviorViewModel CustomerBehaviorData { get; set; } = new CustomerBehaviorViewModel();
    }

    public class ConversionRateViewModel
    {
        public int TotalCartAdditions { get; set; }
        public int TotalPurchases { get; set; }
        public double ConversionRatePercentage { get; set; }
    }

    public class LastMonthViewModel
    {
        public int OrdersCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CustomerBehaviorViewModel
    {
        public string AverageTimeBetweenOrdersFormatted { get; set; } = string.Empty;
        public int AverageTimeBetweenOrdersInSeconds { get; set; }
    }
}