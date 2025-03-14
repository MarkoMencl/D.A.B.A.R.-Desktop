namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ImageAdCollection")]
    public partial class ImageAdCollection
    {
        public int id { get; set; }

        public int ad_id { get; set; }

        public int image_id { get; set; }

        public virtual Ad Ad { get; set; }

        public virtual Image Image { get; set; }
    }
}
