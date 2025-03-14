using EntitiesLayer.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DataAcccessLayer
{
    public partial class DabarModel : DbContext
    {
        public DabarModel()
            : base("name=DabarModel")
        {
        }

        public virtual DbSet<Ad> Ads { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<FavoriteAdCollection> FavoriteAdCollections { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<ImageAdCollection> ImageAdCollections { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ad>()
                .Property(e => e.title)
                .IsUnicode(false);

            modelBuilder.Entity<Ad>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Ad>()
                .HasMany(e => e.FavoriteAdCollections)
                .WithRequired(e => e.Ad)
                .HasForeignKey(e => e.ad_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ad>()
                .HasMany(e => e.ImageAdCollections)
                .WithRequired(e => e.Ad)
                .HasForeignKey(e => e.ad_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.img)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.localizationkey)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.Ads)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.category_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Chat>()
                .Property(e => e.chat_content)
                .IsUnicode(false);

            modelBuilder.Entity<Image>()
                .Property(e => e.format)
                .IsUnicode(false);

            modelBuilder.Entity<Image>()
                .HasMany(e => e.ImageAdCollections)
                .WithRequired(e => e.Image)
                .HasForeignKey(e => e.image_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Image>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.Image)
                .HasForeignKey(e => e.image_id)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Location>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Rating>()
                .Property(e => e.comment)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.contact)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.language)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Ads)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Chats)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.user_id_sender)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Chats1)
                .WithRequired(e => e.User1)
                .HasForeignKey(e => e.user_id_receiver)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FavoriteAdCollections)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Ratings)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.user_id_rater)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Ratings1)
                .WithRequired(e => e.User1)
                .HasForeignKey(e => e.user_id_ratee)
                .WillCascadeOnDelete(false);
        }
    }
}
