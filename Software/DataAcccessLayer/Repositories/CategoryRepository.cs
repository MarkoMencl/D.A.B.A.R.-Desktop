using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcccessLayer.Repositories
{
    public class CategoryRepository : Repository<Category>
    {
        public CategoryRepository() : base(new DabarModel())
        {
        }

        public override async Task<List<Category>> GetAllAsync()
        {
            var query = Context.Categories;
            Console.WriteLine("Categories:" + query);
            return await query.ToListAsync();
        }

        public override int Update(Category entity, bool saveChanges = true)
        {
            throw new NotImplementedException();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            try
            {
                return await Context.Categories.FirstOrDefaultAsync(c => c.id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category with ID {id}: {ex.Message}");
                return null;
            }
        }
    }
}
