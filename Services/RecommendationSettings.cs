namespace TravelPlanner.Application
{
    public class RecommendationSettings
    {
        public int MaxRecommendations { get; set; } = 10;
        public bool Personalized { get; set; } = true;
    }
}