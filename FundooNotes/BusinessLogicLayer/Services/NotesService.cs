using FundooNotes.BusinessLogicLayer.Interfaces;
using FundooNotes.DataLogicLayer.Interfaces;
using FundooNotes.Models.DTOs;
using FundooNotes.Models.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FundooNotes.BusinessLogicLayer.Services
{
  public class NotesService : INotesService
  {
    private readonly INotesRepository _notesRepository;
    private readonly IDistributedCache _cache;
    public NotesService(INotesRepository notesRepository, IDistributedCache cache)
    {
      _notesRepository = notesRepository;
      _cache = cache;
    }

    // Create Note
    public async Task<ResponseDTO> CreateNote(NoteDTO dto, int userId)
    {
      var note = new Notes
      {
        UserId = userId,
        Title = dto.Title,
        Description = dto.Description,
        Color = dto.Color ?? "#FFFFFF",
        Image = dto.Image
      };

      await _notesRepository.CreateNote(note);

      // Invalidate cache after create
      await _cache.RemoveAsync($"notes_user_{userId}");

      return new ResponseDTO
      {
        Success = true,
        Message = "Note created successfully",
        Data = note
      };
    }

    // Get All Notes - filters archived and trashed
    public async Task<ResponseDTO> GetAllNotes(int userId)
    {
      string cacheKey = $"notes_user_{userId}";

      var cachedNotes = await _cache.GetStringAsync(cacheKey);

      if (cachedNotes != null)
      {
        // Step 2 - Return cached data
        var cachedData = JsonSerializer.Deserialize<List<Notes>>(cachedNotes);
        return new ResponseDTO
        {
          Success = true,
          Message = "Notes fetched from cache",
          Data = cachedData
        };
      }

      // Step 3 - Get from DB
      var notes = await _notesRepository.GetAllNotes(userId);

      var activeNotes = notes
          .Where(n => !n.IsArchived && !n.IsTrashed)
          .ToList();

      // Step 4 - Save to cache
      await _cache.SetStringAsync(
          cacheKey,
          JsonSerializer.Serialize(activeNotes),
          new DistributedCacheEntryOptions
          {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
          });

      // Step 5 - Return data
      return new ResponseDTO
      {
        Success = true,
        Message = "Notes fetched successfully",
        Data = activeNotes
      };
    }

    // Get Note by ID
    public async Task<ResponseDTO> GetNoteById(int noteId, int userId)
    {
      var note = await _notesRepository.GetNoteById(noteId, userId);

      if (note == null)
        return new ResponseDTO
        {
          Success = false,
          Message = "Note not found",
          Data = null
        };

      return new ResponseDTO
      {
        Success = true,
        Message = "Note fetched successfully",
        Data = note
      };
    }

    // Update Note
    public async Task<ResponseDTO> UpdateNote(int noteId, NoteDTO dto, int userId)
    {
      var note = await _notesRepository.GetNoteById(noteId, userId);

      // Invalidate cache after create
      await _cache.RemoveAsync($"notes_user_{userId}");

      if (note == null)
        return new ResponseDTO
        {
          Success = false,
          Message = "Note not found",
          Data = null
        };

      // Only update fields that are provided
      note.Title = dto.Title ?? note.Title;
      note.Description = dto.Description ?? note.Description;
      note.Color = dto.Color ?? note.Color;
      note.Image = dto.Image ?? note.Image;

      await _notesRepository.UpdateNote(note);

      return new ResponseDTO
      {
        Success = true,
        Message = "Note updated successfully",
        Data = note
      };
    }

    // Delete Note
    public async Task<ResponseDTO> DeleteNote(int noteId, int userId)
    {
      var result = await _notesRepository.DeleteNote(noteId, userId);

      // Invalidate cache after create
      await _cache.RemoveAsync($"notes_user_{userId}");

      if (!result)
        return new ResponseDTO
        {
          Success = false,
          Message = "Note not found",
          Data = null
        };

      return new ResponseDTO
      {
        Success = true,
        Message = "Note deleted successfully",
        Data = null
      };
    }

    // Trash Note
    public async Task<ResponseDTO> TrashNote(int noteId, int userId)
    {
      var note = await _notesRepository.TrashNote(noteId, userId);

      // Invalidate cache after create
      await _cache.RemoveAsync($"notes_user_{userId}");

      if (note == null)
        return new ResponseDTO
        {
          Success = false,
          Message = "Note not found",
          Data = null
        };

      return new ResponseDTO
      {
        Success = true,
        Message = "Note moved to trash",
        Data = note
      };
    }

    // Archive Note
    public async Task<ResponseDTO> ArchiveNote(int noteId, int userId)
    {
      var note = await _notesRepository.ArchiveNote(noteId, userId);

      if (note == null)
        return new ResponseDTO
        {
          Success = false,
          Message = "Note not found",
          Data = null
        };

      return new ResponseDTO
      {
        Success = true,
        Message = "Note archived successfully",
        Data = note
      };
    }

    // Recover Note from Trash
    public async Task<ResponseDTO> RecoverNote(int noteId, int userId)
    {
      var note = await _notesRepository.RecoverNote(noteId, userId);
      // Invalidate cache after create
      await _cache.RemoveAsync($"notes_user_{userId}");

      if (note == null)
        return new ResponseDTO
        {
          Success = false,
          Message = "Note not found",
          Data = null
        };

      return new ResponseDTO
      {
        Success = true,
        Message = "Note recovered successfully",
        Data = note
      };
    }

    public async Task<ResponseDTO> UnArchiveNote(int noteId, int userId)
    {
      var note = await _notesRepository.UnArchiveNote(noteId, userId);

      // Invalidate cache after create
      await _cache.RemoveAsync($"notes_user_{userId}");

      if (note == null)
        return new ResponseDTO
        {
          Success = false,
          Message = "Note not found",
          Data = null
        };

      return new ResponseDTO
      {
        Success = true,
        Message = "Note unarchived successfully",
        Data = note
      };
    }

  }
}
