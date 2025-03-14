using System;

namespace EntitiesLayer.HelperEntities
{
    public class AdDTOExtended
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageBase64 { get; set; }
        public int UserId { get; set; }
        public int Views { get; set; }
        public double Price { get; set; }
        public DateTime Date_of_publication { get; set; }
        public int CategoryId { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
    }
}
