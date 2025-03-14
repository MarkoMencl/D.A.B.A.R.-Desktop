using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class LocationService
    {
        public List<Location> GetLocations()
        {
            using (var repo = new LocationRepository())
            {
                List<Location> locations = repo.GetAllAsync().Result.ToList();
                return locations;
            }
        }

        public async Task<Location> GetLocationByIdAsync(int id)
        {
            using (var repo = new LocationRepository())
            {
                Location location = await repo.GetLocationByIdAsync(id);
                return location;
            }
        }

    }
}
