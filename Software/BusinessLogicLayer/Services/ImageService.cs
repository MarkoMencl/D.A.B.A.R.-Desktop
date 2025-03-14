using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;

namespace BusinessLogicLayer.Services
{
    public class ImageService
    {
        private readonly ImageRepository imageRepository;

        public ImageService()
        {
            imageRepository = new ImageRepository();
        }

        public int SaveImage(Image image)
        {
            if (image.id > 0)
            {
                var existingImage = imageRepository.GetImageById(image.id);
                if (existingImage != null)
                {
                    existingImage.bitmap = image.bitmap;
                    existingImage.format = image.format;
                    existingImage.size = image.size;
                    imageRepository.Update(existingImage);
                    imageRepository.SaveChanges();
                    return existingImage.id;
                }
            }

            imageRepository.Insert(image);
            imageRepository.SaveChanges();

            return image.id;
        }


        public Image GetUserImage(int imageId)
        {
            return imageRepository.GetImageById(imageId);
        }

        public int? GetLastInsertedImageId(int id)
        {
            return imageRepository.GetLastInsertedImageId(id);
        }
    }
}
