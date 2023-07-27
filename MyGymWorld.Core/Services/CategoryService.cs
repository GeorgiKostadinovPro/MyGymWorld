namespace MyGymWorld.Core.Services
{
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CategoryService : ICategoryService
    {
        private readonly IRepository repository;

        public CategoryService(IRepository _repository)
        {
            this.repository = _repository;
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await this.repository.AllReadonly<Category>(c => c.IsDeleted == false)
                .ToArrayAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(string categoryId)
        {
            return await this.repository.AllReadonly<Category>(c => c.IsDeleted == false && c.Id == Guid.Parse(categoryId))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync()
		{
			return await this.repository.AllReadonly<Category>()
				.Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString()
				})
				.OrderBy(sli => sli.Text)
				.ToArrayAsync();
		}
    }
}
