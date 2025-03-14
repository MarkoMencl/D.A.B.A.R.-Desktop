using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Managers;
using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using EntitiesLayer.HelperEntities;


namespace BusinessLogicLayer.Services
{
    public class AdService
    {
        public bool DeleteAd(int adId)
        {
            using (var repo = new AdRepository())
            {
                return repo.DeleteAd(adId);
            }
        }

        public List<Ad> GetUserAds(int userId)
        {
            using (var repo = new AdRepository())
            {
                return repo.GetUserAds(userId).ToList();    
            }
        }

        public bool PostAd(string title, string description, double price, int categoryId, int status, byte[] imageBytes, string imageFormat, int? imageSize)
        {
            using (var repo = new AdRepository())
            {
                var user = SessionManager.GetCurrentUser();
                if (user == null)
                {
                    return false;
                }

                Ad ad = new Ad
                {
                    title = title,
                    description = description,
                    price = price,
                    date_of_publication = DateTime.Now,
                    status = status,
                    seller_confirm = 0,
                    buyer_confirm = 0,
                    user_id = user.id,
                    views = 0,
                    category_id = categoryId
                };

                int adId = repo.AddAd(ad);

                if (adId > 0 && imageBytes != null && imageBytes.Length > 0)
                {
                    using (var imageRepo = new ImageRepository())
                    {
                        Image image = new Image
                        {
                            bitmap = imageBytes,
                            format = imageFormat,
                            size = imageSize
                        };

                        int imageId = imageRepo.Insert(image);
                        

                        if (imageId > 0)
                        {
                            repo.LinkImageToAd(adId, imageId);
                        }
                    }
                }

                return true;
            }
        }

        public async Task<List<AdDTO>> GetAllAdsAsync()
        {
            using (var repo = new AdRepository())
            {
                List<Ad> ads = await repo.GetAllAsync();

                using (var imageRepo = new ImageRepository())
                {
                    List<AdDTO> adDTOs = new List<AdDTO>();

                    foreach (var ad in ads)
                    {
                        var firstImage = await imageRepo.GetFirstImageByAdIdAsync(ad.id);
                        string imageBase64 = firstImage?.bitmap != null
                            ? $"data:image/png;base64,{Convert.ToBase64String(firstImage.bitmap)}"
                            : "default_image_path";

                        adDTOs.Add(new AdDTO
                        {
                            Id = ad.id,
                            Title = ad.title,
                            Description = ad.description,
                            ImageBase64 = imageBase64
                        });
                    }

                    return adDTOs;
                }
            }
        }

        public Ad GetAdByIdAsync(int adId)
        {
            using (var repo = new AdRepository())
            {
                var ad = repo.GetAdById(adId);

                if (ad != null)
                {
                    using (var imageRepo = new ImageRepository())
                    {
                        var images = imageRepo.GetImagesByAdId(ad.id);
                        ad.ImageAdCollections = images.Select(image => new ImageAdCollection
                        {
                            image_id = image.id,
                            ad_id = ad.id,
                            Image = image
                        }).ToList();
                    }

                    if (ad.User != null)
                    {
                        ad.User.contact = ad.User.contact;
                        ad.User.email = ad.User.email;
                        ad.User.username = ad.User.username;
                        ad.User.location_id = ad.User.location_id;
                        ad.User.Image = ad.User.Image;
                    }
                }

                return ad;
            }
        }


        public async Task<bool> UpdateAdAsync(int adId, string title, string description, double price, int categoryId, int status, byte[] imageBytes, string imageFormat, int? imageSize)
        {
            using (var repo = new AdRepository())
            {
                Ad ad = repo.GetAdById(adId);
                if (ad == null)
                    return false;

                ad.title = title;
                ad.description = description;
                ad.price = price;
                ad.category_id = categoryId;
                ad.status = status;

                int updateResult = repo.Update(ad);
                if (updateResult <= 0)
                    return false;

                if (imageBytes != null && imageBytes.Length > 0)
                {
                    using (var imageRepo = new ImageRepository())
                    {
                        var existingImage = await imageRepo.GetFirstImageByAdIdAsync(adId).ConfigureAwait(false);
                        if (existingImage != null)
                        {
                            existingImage.bitmap = imageBytes;
                            existingImage.format = imageFormat;
                            existingImage.size = imageSize;
                            if (imageRepo.Update(existingImage) <= 0)
                                return false;
                        }
                        else
                        {
                            Image newImage = new Image
                            {
                                bitmap = imageBytes,
                                format = imageFormat,
                                size = imageSize
                            };
                            int newImageId = imageRepo.Insert(newImage);
                            if (newImageId > 0)
                                repo.LinkImageToAd(adId, newImageId);
                        }
                    }
                }
                return true;
            }
        }

        public IEnumerable GetAdsByQuery(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AdDTOExtended>> GetAllAdsExtendedAsync()
        {
            using (var repo = new AdRepository())
            {
                List<Ad> ads = await repo.GetAllAsync();
                using (var imageRepo = new ImageRepository())
                {
                    List<AdDTOExtended> adDTOs = new List<AdDTOExtended>();
                    foreach (var ad in ads)
                    {
                        var firstImage = await imageRepo.GetFirstImageByAdIdAsync(ad.id);
                        string imageBase64 = firstImage?.bitmap != null ? $"data:image/png;base64,{Convert.ToBase64String(firstImage.bitmap)}" : "default_image_path";
                        int locationId = 0;
                        string location = string.Empty;
                        if (ad.User != null && ad.User.location_id.HasValue)
                        {
                            locationId = ad.User.location_id.Value;
                            location = LocaleManager.GetLocalizedString("Location_" + locationId);
                        }
                        adDTOs.Add(new AdDTOExtended
                        {
                            Id = ad.id,
                            Title = ad.title,
                            Description = ad.description,
                            ImageBase64 = imageBase64,
                            UserId = ad.user_id,
                            Views = ad.views ?? 0,
                            Price = ad.price,
                            Date_of_publication = ad.date_of_publication ?? DateTime.MinValue,
                            CategoryId = ad.category_id,
                            LocationId = locationId,
                            Location = location
                        });
                    }
                    return adDTOs;
                }
            }
        }
        public async Task<List<AdDTOExtended>> GetFilteredSortedAdsAsync(string query, int? categoryId, int? locationIdFilter, int sortOption)
        {
            var allAds = await GetAllAdsExtendedAsync();

            if (!string.IsNullOrWhiteSpace(query))
                allAds = allAds.Where(a => a.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                            a.Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (categoryId.HasValue)
                allAds = allAds.Where(a => a.CategoryId == categoryId.Value).ToList();
            if (locationIdFilter.HasValue)
                allAds = allAds.Where(a => a.LocationId == locationIdFilter.Value).ToList();

            switch (sortOption)
            {
                case 0:
                    break;
                case 1:
                    allAds = allAds.OrderByDescending(a => a.Views).ToList();
                    break;
                case 2:
                    allAds = allAds.OrderBy(a => a.Price).ToList();
                    break;
                case 3:
                    allAds = allAds.OrderByDescending(a => a.Price).ToList();
                    break;
                case 4:
                    allAds = allAds.OrderByDescending(a => a.Date_of_publication).ToList();
                    break;
                case 5:
                    allAds = allAds.OrderBy(a => a.Date_of_publication).ToList();
                    break;
                default:
                    break;
            }

            return allAds;
        }

        public async Task<List<Ad>> GetUserAdsAsync(int userId)
        {
            using (var repo = new AdRepository())
            {
                return await Task.Run(() => repo.GetUserAds(userId).ToList());
            }
        }

        
        public async Task<bool> IncrementAdViewsAsync(int adId)
        {
            using (var repo = new AdRepository())
            {
                Ad ad = repo.GetAdById(adId);
                if (ad == null)
                    return false;
                ad.views = (ad.views ?? 0) + 1;
                int result = repo.Update(ad);
                return result > 0;
            }
        }
    }
}

