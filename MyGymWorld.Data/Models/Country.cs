namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class Country : BaseDeletableEntityModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
