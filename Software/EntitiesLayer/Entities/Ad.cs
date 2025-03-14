namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Ad")]
    public partial class Ad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ad()
        {
            FavoriteAdCollections = new HashSet<FavoriteAdCollection>();
            ImageAdCollections = new HashSet<ImageAdCollection>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(200)]
        public string title { get; set; }

        [Required]
        [StringLength(500)]
        public string description { get; set; }

        public double price { get; set; }

        public int user_id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_of_publication { get; set; }

        public int status { get; set; }

        public byte seller_confirm { get; set; }

        public byte buyer_confirm { get; set; }

        public int? views { get; set; }

        public int category_id { get; set; }

        public virtual Category Category { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FavoriteAdCollection> FavoriteAdCollections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageAdCollection> ImageAdCollections { get; set; }
    }
}
