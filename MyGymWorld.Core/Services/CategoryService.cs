namespace MyGymWorld.Core.Services
{
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
    }
}
