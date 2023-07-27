namespace MyGymWorld.Web.ViewModels.Articles
{
    public class AllArticlesForGymFilteredAndPagedViewModel
    {
        public AllArticlesForGymFilteredAndPagedViewModel()
        {
            this.Articles = new HashSet<ArticleViewModel>();
        }

        public int TotalArticlesCount { get; set; }

        public IEnumerable<ArticleViewModel> Articles { get; set; }
    }
}
