namespace Mockups.Models.Analytics
{
    public class AnalyticsViewModel
    {
        public ConversionRateViewModel ConversionRateData { get; set; } = new ConversionRateViewModel();
    }

    public class ConversionRateViewModel
    {
        public int TotalCartAdditions { get; set; }
        public int TotalPurchases { get; set; }
        public double ConversionRatePercentage { get; set; }
    }
}