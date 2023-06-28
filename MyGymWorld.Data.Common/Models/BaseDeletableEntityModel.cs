namespace MyGymWorld.Data.Common.Models
{
    using MyGymWorld.Data.Common.Contracts;
    using System;

    /// <summary>
    /// This is my custom abstract class for adding two more properties to the entity - IsDeleted and DeletedOn.
    /// It inhertis the BaseDeletableModel abstract class and additional interface - IDeletableModel.
    /// Use this abstract class whenever you want to have Deletable properties but also not lose the CreatedOn and ModifiedOn properties.
    /// </summary>
    public abstract class BaseDeletableEntityModel : BaseEntityModel, IDeletableModel
    {
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}