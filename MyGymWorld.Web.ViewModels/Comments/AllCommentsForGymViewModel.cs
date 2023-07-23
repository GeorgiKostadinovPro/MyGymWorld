namespace MyGymWorld.Web.ViewModels.Comments
{
    using System.Collections.Generic;

    public class AllCommentsForGymViewModel
    {
        public AllCommentsForGymViewModel()
        {
            this.Comments = new List<CommentViewModel>();
        }

        public string GymId { get; set; } = null!; 
        
        public string Name { get; set; } = null!;
 
        public int CurrentPage { get; set; }

        public int PagesCount { get; set; }

        public List<CommentViewModel> Comments { get; set; }
    }
}
