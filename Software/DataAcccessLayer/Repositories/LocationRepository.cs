using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcccessLayer.Repositories
{
    public class LocationRepository : Repository<Location>
    {
        public LocationRepository() : base(new DabarModel())
        {

        }

        public async Task<Location> GetLocationByIdAsync(int id)
        {
            var locations = await GetAllAsync();
            return locations.FirstOrDefault(x => x.id == id);
        }

        public override Task<List<Location>> GetAllAsync()
        {
            var query = Context.Locations;
            return query.ToListAsync();
        }

        public override int Update(Location entity, bool saveChanges = true)
        {
            throw new NotImplementedException();
        }
    }
}
