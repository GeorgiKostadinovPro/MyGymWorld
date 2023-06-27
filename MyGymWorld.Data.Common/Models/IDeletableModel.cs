namespace MyGymWorld.Data.Common.Contracts
{
    public interface IDeletableModel
    {
        bool IsDeleted { get; set; }

        DateTime? DeletedOn { get; set; }
    }
}
