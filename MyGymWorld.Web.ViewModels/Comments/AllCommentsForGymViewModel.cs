namespace MyGymWorld.Web.ViewModels.Comments
{
    using System.Collections.Generic;

    public class AllCommentsForGymViewModel
    {
        public AllCommentsForGymViewModel()
        {
            this.Comments = new HashSet<CommentViewModel>();
        }

        public string GymId { get; set; } = null!; 
        
        public string Name { get; set; } = null!;
 
        public int CurrentPage { get; set; }

        public int PagesCount { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
