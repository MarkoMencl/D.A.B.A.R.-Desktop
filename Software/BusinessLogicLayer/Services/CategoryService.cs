using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CategoryService
    {
        public async Task<List<Category>> GetCategoriesAsync()
        {
            using (var repo = new CategoryRepository())
            {
                List<Category> categories = await repo.GetAllAsync();
                Console.WriteLine("Categories:" + categories);
                return categories;
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            using (var repo = new CategoryRepository())
            {
                var category = await repo.GetByIdAsync(categoryId);
                Console.WriteLine($"Fetched Category: {category?.name ?? "Not Found"} for ID: {categoryId}");
                return category;
            }
        }
    }
}
