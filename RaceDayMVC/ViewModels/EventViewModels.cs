using Microsoft.AspNetCore.Http;


namespace RaceDay.MVC.ViewModels
{
    public class CreateEventViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public EventType EventType { get; set; }
        public IFormFile? BannerImage { get; set; } // This handles the file upload
    }
}