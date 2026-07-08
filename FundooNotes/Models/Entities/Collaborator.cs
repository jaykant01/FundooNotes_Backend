namespace FundooNotes.Models.Entities
{
    public class Collaborator
    {
        public int CollaboratorId { get; set; }

        public int NoteId { get; set; }

        public int OwnerId { get; set; }

        public required string CollaboratorEmail { get; set; }

        // Navigation
        public Notes? Note { get; set; }

        public User? Owner { get; set; }
    }
}
