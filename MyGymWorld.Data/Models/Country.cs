namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    /// <summary>
    /// Country model.
    /// All validation including primary key and foreign key constraints are described with FluentAPI.
    /// View CountryConfiguration class in the Configuration folder.
    /// </summary>
    public class Country : BaseDeletableEntityModel
    {
        public Country()
        {
            this.Id = Guid.NewGuid();

            this.Towns = new HashSet<Town>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Town> Towns { get; set; }
    }
}
