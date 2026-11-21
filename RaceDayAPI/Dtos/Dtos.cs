using RaceDay.Domain;
using System.Text.Json.Serialization; // Add this using statement

namespace RaceDay.API.Dtos;

// ==========================================
// AUTHENTICATION ENVELOPES (Register & Login)
// ==========================================
public record RegisterDto(
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string LastName,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("role")] UserRole Role);

public record LoginDto(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password);

public record LoginResponse(
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("userId")] int UserId,
    [property: JsonPropertyName("firstName")] string FirstName);

// ... Keep the rest of your DTOs exactly as they are ...

// ==========================================
// USER PROFILE ENVELOPES
// ==========================================
public record UserDto(
    int Id, string FirstName, string LastName, string Email,
    UserRole Role, string? PhoneNumber, DateTime? DateOfBirth,
    string? ProfilePictureUrl);

public record UpdateProfileDto(
    string FirstName, string LastName,
    string? PhoneNumber, DateTime? DateOfBirth,
    string? ProfilePictureUrl);

// ==========================================
// EVENT ENVELOPES
// ==========================================
public record EventDto(
    int Id, string Name, string Description, DateTime EventDate,
    string Location, decimal DistanceKm, EventType EventType,
    string? BannerImageUrl, int OrganiserId, string OrganiserName);

public record CreateEventDto(
    string Name, string Description, DateTime EventDate,
    string Location, decimal DistanceKm, EventType EventType,
    string? BannerImageUrl);

public record UpdateEventDto(
    string Name, string Description, DateTime EventDate,
    string Location, decimal DistanceKm, EventType EventType,
    string? BannerImageUrl);

// ==========================================
// CATEGORY ENVELOPES
// ==========================================
public record CategoryDto(
    int Id, int EventId, string Name,
    int? MinAge, int? MaxAge, decimal? DistanceKm);

public record CreateCategoryDto(
    string Name, int? MinAge, int? MaxAge, decimal? DistanceKm);

// ==========================================
// ENROLMENT ENVELOPES
// ==========================================
public record EnrolmentDto(
    int Id, int ParticipantId, string ParticipantName,
    int EventId, string EventName, int CategoryId, string CategoryName,
    EnrolmentStatus Status, DateTime EnrolledAt);

public record CreateEnrolmentDto(int CategoryId);

// ==========================================
// RESULT ENVELOPES
// ==========================================
public record ResultDto(
    int Id, int EnrolmentId, int ParticipantId, string ParticipantName,
    int EventId, string EventName, string CategoryName,
    TimeSpan FinishTime, int Position, DateTime CapturedAt);

public record CaptureResultDto(
    TimeSpan FinishTime, int Position);