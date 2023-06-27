namespace MyGymWorld.Data.Common.Contracts
{
    public interface ITimestampableModel
    {
        DateTime CreatedOn { get; set; }

        DateTime? ModifiedOn { get; set; }
    }
}
