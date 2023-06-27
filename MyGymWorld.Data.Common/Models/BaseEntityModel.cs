namespace MyGymWorld.Data.Common.Models
{
    using MyGymWorld.Data.Common.Contracts;
    using System;

    /// <summary>
    /// This is my custom abstract class for adding initial properties CreatedOn and ModifiedOn.
    /// Inherit this class whenever you want to have these two properties.
    /// </summary>
    public abstract class BaseEntityModel : ITimestampableModel
    {
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
