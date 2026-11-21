using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Auth;
using RaceDay.API.Dtos;
using RaceDay.API.Services;
using RaceDay.Domain;
using RaceDay.Infrastructure;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly BlobService _blobService; // Added this

    public EventsController(RaceDayDbContext db, BlobService blobService)
    {
        _db = db;
        _blobService = blobService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EventType? type)
    {
        var query = _db.Events.Include(e => e.Organiser).AsQueryable();
        if (type.HasValue) query = query.Where(e => e.EventType == type.Value);
        var events = await query.OrderBy(e => e.EventDate).ToListAsync();
        return Ok(events.Select(ToDto));
    }

    [HttpGet("mine")]
    [RequireRole(UserRole.Organiser)]
    public async Task<IActionResult> GetMine()
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        var events = await _db.Events
            .Include(e => e.Organiser)
            .Where(e => e.OrganiserId == userId)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
        return Ok(events.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var ev = await _db.Events.Include(e => e.Organiser).FirstOrDefaultAsync(e => e.Id == id);
        return ev is null ? NotFound() : Ok(ToDto(ev));
    }

    // UPDATED: Accepts Form Data and Image File
    [HttpPost]
    [RequireRole(UserRole.Organiser)]
    public async Task<IActionResult> Create([FromForm] CreateEventDto dto, [FromForm] IFormFile? bannerImage)
    {
        var userId = (int)HttpContext.Items["UserId"]!;

        string? imageUrl = null;
        if (bannerImage != null && bannerImage.Length > 0)
        {
            imageUrl = await _blobService.UploadFileAsync(bannerImage);
        }

        var ev = new Event
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            EventDate = dto.EventDate,
            Location = dto.Location.Trim(),
            DistanceKm = dto.DistanceKm,
            EventType = dto.EventType,
            BannerImageUrl = imageUrl, // Save Azurite URL
            OrganiserId = userId
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        await _db.Entry(ev).Reference(e => e.Organiser).LoadAsync();

        return CreatedAtAction(nameof(GetOne), new { id = ev.Id }, ToDto(ev));
    }

    // UPDATED: Accepts Form Data and Image File
    [HttpPut("{id:int}")]
    [RequireRole(UserRole.Organiser)]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateEventDto dto, [FromForm] IFormFile? bannerImage)
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.Include(e => e.Organiser).FirstOrDefaultAsync(e => e.Id == id);

        if (ev is null) return NotFound();
        if (ev.OrganiserId != userId) return Forbid();

        // If a new image is uploaded, replace the old one
        if (bannerImage != null && bannerImage.Length > 0)
        {
            ev.BannerImageUrl = await _blobService.UploadFileAsync(bannerImage);
        }

        ev.Name = dto.Name.Trim();
        ev.Description = dto.Description.Trim();
        ev.EventDate = dto.EventDate;
        ev.Location = dto.Location.Trim();
        ev.DistanceKm = dto.DistanceKm;
        ev.EventType = dto.EventType;

        await _db.SaveChangesAsync();
        return Ok(ToDto(ev));
    }

    [HttpDelete("{id:int}")]
    [RequireRole(UserRole.Organiser)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != userId) return Forbid();
        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static EventDto ToDto(Event e) => new(
        e.Id, e.Name, e.Description, e.EventDate, e.Location, e.DistanceKm,
        e.EventType, e.BannerImageUrl, e.OrganiserId,
        e.Organiser is null ? "Unknown" : $"{e.Organiser.FirstName} {e.Organiser.LastName}");
}