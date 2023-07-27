namespace MyGymWorld.Web.ViewModels.Articles
{
	public class ArticleDetailsViewModel
	{
		public string Id { get; set; } = null!;

		public string Title { get; set; } = null!;

		public string Content { get; set; } = null!;

		public string GymId { get; set; } = null!;

		public string GymName { get; set; } = null!;

		public string LogoUri { get; set; } = null!;

		public DateTime CreatedOn { get; set; }
	}
}
