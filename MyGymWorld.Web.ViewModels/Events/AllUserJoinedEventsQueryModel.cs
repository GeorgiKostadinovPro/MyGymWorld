namespace MyGymWorld.Web.ViewModels.Events
{
    using MyGymWorld.Web.ViewModels.Events.Enums;
    using System.ComponentModel.DataAnnotations;
    using static MyGymWorld.Common.GlobalConstants;

    public class AllUserJoinedEventsQueryModel : AllEventsForGymQueryModel
    {
        public AllUserJoinedEventsQueryModel()
        {
        }

        public string UserId { get; set; } = null!;
    }
}
