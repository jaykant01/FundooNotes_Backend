using FundooNotes.BusinessLogicLayer.Interfaces;
using FundooNotes.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class NotesController : ControllerBase
  {
    private readonly INotesService _notesService;

    public NotesController(INotesService notesService)
    {
      _notesService = notesService;
    }

    // Helper method to get userId from JWT token
    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    // Create Note
    [HttpPost("create")]
    public async Task<IActionResult> CreateNote(NoteDTO dto)
    {
      var result = await _notesService.CreateNote(dto, GetUserId());
      if (result.Success)
        return Ok(result);
      return BadRequest(result);
    }

    // Get All Notes
    [HttpGet("getall")]
    public async Task<IActionResult> GetAllNotes()
    {
      var result = await _notesService.GetAllNotes(GetUserId());
      if (result.Success)
        return Ok(result);
      return BadRequest(result);
    }

    // Get Note by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNoteById(int id)
    {
      var result = await _notesService.GetNoteById(id, GetUserId());
      if (result.Success)
        return Ok(result);
      return NotFound(result);
    }

    // Update Note
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateNote(int id, NoteDTO dto)
    {
      var result = await _notesService.UpdateNote(id, dto, GetUserId());
      if (result.Success)
        return Ok(result);
      return NotFound(result);
    }

    // Delete Note
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
      var result = await _notesService.DeleteNote(id, GetUserId());
      if (result.Success)
        return Ok(result);
      return NotFound(result);
    }

    // Trash Note
    [HttpPut("trash/{id}")]
    public async Task<IActionResult> TrashNote(int id)
    {
      var result = await _notesService.TrashNote(id, GetUserId());
      if (result.Success)
        return Ok(result);
      return NotFound(result);
    }

    // Archive Note
    [HttpPut("archive/{id}")]
    public async Task<IActionResult> ArchiveNote(int id)
    {
      var result = await _notesService.ArchiveNote(id, GetUserId());
      if (result.Success)
        return Ok(result);
      return NotFound(result);
    }


    [HttpPut("unarchive/{id}")]
    public async Task<IActionResult> UnArchiveNote(int id)
    {
      var result = await _notesService.UnArchiveNote(id, GetUserId());
      if (result.Success)
        return Ok(result);
      return NotFound(result);
    }

    // Recover Trash Note
    [HttpPut("recover/{id}")]
    public async Task<IActionResult> RecoverNote(int id)
    {
      var result = await _notesService.RecoverNote(id, GetUserId());
      if (result.Success)
        return Ok(result);
      return NotFound(result);
    }
  }
}
