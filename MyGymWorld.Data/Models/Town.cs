namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class Town : BaseDeletableEntityModel
    {
        public Town()
        {
            this.Id = Guid.NewGuid();

            this.Addresses = new HashSet<Address>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public int Population { get; set; }

        public string ZipCode { get; set; } = null!;

        public Guid CountryId { get; set; }

        public virtual Country Country { get; set; } = null!;

        public virtual ICollection<Address> Addresses { get; set; }
    }
}
