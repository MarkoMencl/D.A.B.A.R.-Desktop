namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("FavoriteAdCollection")]
    public partial class FavoriteAdCollection
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public int ad_id { get; set; }

        public virtual Ad Ad { get; set; }

        public virtual User User { get; set; }
    }
}
