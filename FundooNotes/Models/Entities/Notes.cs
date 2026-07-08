namespace FundooNotes.Models.Entities
{
  public class Notes
  {
    public int NoteId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Color { get; set; } = "#FFFFFF";

    public string? Image { get; set; }

    public bool IsArchived { get; set; } = false;

    public bool IsTrashed { get; set; } = false;

    public bool IsPinned { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedAt { get; set; }

    // Navigation Properties

    public User? User { get; set; }

    public ICollection<Label>? Labels { get; set; }

    public ICollection<Collaborator>? Collaborators { get; set; }


  }
}
