using FundooNotes.Models.DTOs;

namespace FundooNotes.BusinessLogicLayer.Interfaces
{
  public interface INotesService
  {
    Task<ResponseDTO> CreateNote(NoteDTO dto, int userId);
    Task<ResponseDTO> GetAllNotes(int userId);
    Task<ResponseDTO> GetNoteById(int noteId, int userId);
    Task<ResponseDTO> UpdateNote(int noteId, NoteDTO dto, int userId);
    Task<ResponseDTO> DeleteNote(int noteId, int userId);
    Task<ResponseDTO> TrashNote(int noteId, int userId);
    Task<ResponseDTO> ArchiveNote(int noteId, int userId);
    Task<ResponseDTO> RecoverNote(int noteId, int userId);
    Task<ResponseDTO> UnArchiveNote(int noteId, int userId);
  }
}
