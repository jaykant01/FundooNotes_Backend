using FundooNotes.DataLogicLayer.Context;
using FundooNotes.DataLogicLayer.Interfaces;
using FundooNotes.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundooNotes.DataLogicLayer.Repositories
{
  public class NotesRepository : INotesRepository
  {
    private readonly FundooContext _context;

    public NotesRepository(FundooContext context)
    {
      _context = context;
    }

    public async Task<Notes?> ArchiveNote(int noteId, int userId)
    {
      var note = await _context.Notes
          .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

      if (note == null) return null;

      note.IsArchived = true;
      note.IsTrashed = false;  // untrash if trashed
      note.ModifiedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
      return note;
    }

    public async Task<Notes> CreateNote(Notes note)
    {
      _context.Notes.Add(note);
      await _context.SaveChangesAsync();
      return note;
    }

    public async Task<bool> DeleteNote(int noteId, int userId)
    {
      var note = await _context.Notes
          .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

      if (note == null) return false;

      _context.Notes.Remove(note);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<List<Notes>> GetAllNotes(int userId)
    {
      return await _context.Notes
         .Where(n => n.UserId == userId)
         .ToListAsync();
    }

    public async Task<Notes?> GetNoteById(int noteId, int userId)
    {
      return await _context.Notes
          .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);
    }

    public async Task<Notes?> RecoverNote(int noteId, int userId)
    {
      var note = await _context.Notes
          .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

      if (note == null) return null;

      note.IsTrashed = false;
      note.ModifiedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
      return note;
    }

    public async Task<Notes?> TrashNote(int noteId, int userId)
    {
      var note = await _context.Notes
          .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

      if (note == null) return null;

      note.IsTrashed = true;
      note.IsArchived = false;  // unarchive if archived
      note.ModifiedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
      return note;
    }

    public async Task<Notes> UpdateNote(Notes note)
    {
      note.ModifiedAt = DateTime.UtcNow;
      _context.Notes.Update(note);
      await _context.SaveChangesAsync();
      return note;
    }

    public async Task<Notes?> UnArchiveNote(int noteId, int userId)
    {
      var note = await _context.Notes
          .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

      if (note == null) return null;

      note.IsArchived = false;
      note.ModifiedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
      return note;
    }
  }
}
