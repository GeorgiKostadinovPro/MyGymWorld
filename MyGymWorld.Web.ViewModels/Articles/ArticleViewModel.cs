namespace MyGymWorld.Web.ViewModels.Articles
{
    public class ArticleViewModel
    {
        public string Id { get;  set; } = null!;

        public string Title { get; set; } = null!;

        public string ShortContent { get; set; } = null!;

        public string GymId { get; set; } = null!;

        public string GymName { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}
