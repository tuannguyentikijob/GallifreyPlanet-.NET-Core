using GallifreyPlanet.ViewModels.Blog;

namespace GallifreyPlanet.ViewModels
{
    public class PublicProfileViewModel
    {
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? Github { get; set; }
        public string? Twitter { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public RecentActivities RecentActivities { get; set; } = new RecentActivities();
        public List<BlogViewModel> RecentBlogs { get; set; } = new List<BlogViewModel>();
    }

    public class RecentActivities
    {
        public int CommentPercentage { get; set; }
        public int LikePercentage { get; set; }
        public int SharePercentage { get; set; }
        public int RatingPercentage { get; set; }
        public int FollowPercentage { get; set; }
    }
}