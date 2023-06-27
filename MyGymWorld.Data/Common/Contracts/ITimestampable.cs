namespace MyGymWorld.Data.Common.Contracts
{
    public interface ITimestampable
    {
        DateTime CreatedOn { get; set; }

        DateTime? ModifiedOn { get; set; }
    }
}
