namespace TravelPlanner.Application
{
    public class AnalyticsSettings
    {
        public bool Enabled { get; set; } = true;
        public int RetentionDays { get; set; } = 90;
    }
}