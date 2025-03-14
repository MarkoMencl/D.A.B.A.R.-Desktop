namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Location")]
    public partial class Location
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }
    }
}
