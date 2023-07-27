namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;

    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();

        Task<Category?> GetCategoryByIdAsync(string categoryId);
    }
}
