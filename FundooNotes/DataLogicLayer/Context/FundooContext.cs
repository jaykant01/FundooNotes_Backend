using FundooNotes.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundooNotes.DataLogicLayer.Context
{
    public class FundooContext : DbContext
    {
        public FundooContext(DbContextOptions<FundooContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Notes> Notes { get; set; }

        public DbSet<Label> Labels { get; set; }

        public DbSet<Collaborator> Collaborators { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.HashPassword)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.ResetToken)
                .HasMaxLength(200);

            // Notes

            modelBuilder.Entity<Notes>()
                .HasKey(n => n.NoteId);

            modelBuilder.Entity<Notes>()
                .Property(n => n.Title)
                .HasMaxLength(200);

            modelBuilder.Entity<Notes>()
                .Property(n => n.Description)
                .HasMaxLength(5000);

            modelBuilder.Entity<Notes>()
                .Property(n => n.Color)
                .HasDefaultValue("#FFFFFF");

            modelBuilder.Entity<Notes>()
                .Property(n => n.IsArchived)
                .HasDefaultValue(false);

            modelBuilder.Entity<Notes>()
                .Property(n => n.IsPinned)
                .HasDefaultValue(false);

            modelBuilder.Entity<Notes>()
                .Property(n => n.IsTrashed)
                .HasDefaultValue(false);

            modelBuilder.Entity<Notes>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Notes>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notes>()
                .HasMany(n => n.Labels)
                .WithMany(l => l.Notes)
                .UsingEntity(j => j.ToTable("NoteLabels")
                .HasOne(typeof(Notes))
                .WithMany()
                .HasForeignKey("NotesNoteId")
                .OnDelete(DeleteBehavior.NoAction));

            // Label 

            modelBuilder.Entity<Label>()
                .HasKey(l => l.LabelId);

            modelBuilder.Entity<Label>()
                .Property(l => l.LabelName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Label>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Collaborator
            modelBuilder.Entity<Collaborator>()
                .HasKey(c => c.CollaboratorId);

            modelBuilder.Entity<Collaborator>()
                .Property(c => c.CollaboratorEmail)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.Note)
                .WithMany(n => n.Collaborators)
                .HasForeignKey(c => c.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
