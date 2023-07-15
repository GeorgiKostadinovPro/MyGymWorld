namespace MyGymWorld.Data.Models
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Data.Common.Models;
    using System;

    public class GymImage : BaseDeletableEntityModel
    {
        public GymImage()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Uri { get; set; } = null!;

        public string PublicId { get; set; } = null!;

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;
    }
}
