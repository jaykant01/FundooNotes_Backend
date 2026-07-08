using FundooNotes.Models.Entities;

namespace FundooNotes.DataLogicLayer.Interfaces
{
  public interface INotesRepository
  {
    Task<Notes> CreateNote(Notes note);
    Task<List<Notes>> GetAllNotes(int userId);
    Task<Notes?> GetNoteById(int noteId, int userId);
    Task<Notes> UpdateNote(Notes note);
    Task<bool> DeleteNote(int noteId, int userId);
    Task<Notes?> TrashNote(int noteId, int userId);

    Task<Notes?> RecoverNote(int noteId, int userId);
    Task<Notes?> ArchiveNote(int noteId, int userId);
    Task<Notes?> UnArchiveNote(int noteId, int userId);

  }
}
