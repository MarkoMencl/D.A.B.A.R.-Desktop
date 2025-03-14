namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Rating")]
    public partial class Rating
    {
        public int id { get; set; }

        public int value { get; set; }

        [Required]
        [StringLength(2000)]
        public string comment { get; set; }

        public int user_id_rater { get; set; }

        public int user_id_ratee { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
