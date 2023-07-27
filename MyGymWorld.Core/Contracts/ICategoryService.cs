namespace MyGymWorld.Core.Contracts
{
	using Microsoft.AspNetCore.Mvc.Rendering;
	using MyGymWorld.Data.Models;

	public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();

        Task<Category?> GetCategoryByIdAsync(string categoryId);

		Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync();
	}
}
