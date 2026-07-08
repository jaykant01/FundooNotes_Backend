namespace FundooNotes.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public required string FirstName { get; set; }

        public string? LastName { get; set; }

        public required string Email { get; set; }

        public required string HashPassword { get; set; }

        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        public ICollection<Notes>? Notes { get; set; }

    }
}
