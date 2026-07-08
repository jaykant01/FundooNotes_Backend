namespace FundooNotes.Models.Entities
{
    public class Label
    {
        public int LabelId { get; set; }

        public int UserId { get; set; }

        public required string LabelName { get; set; }

        // Navigation

        public User? User { get; set; }

        public ICollection<Notes>? Notes { get; set; }

    }
}
