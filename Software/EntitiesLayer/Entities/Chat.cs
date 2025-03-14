namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Chat")]
    public partial class Chat
    {
        public int id { get; set; }

        public int user_id_sender { get; set; }

        public int user_id_receiver { get; set; }

        [Required]
        [StringLength(5000)]
        public string chat_content { get; set; }

        public DateTime date { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
