using System.ComponentModel.DataAnnotations;

namespace SudanDialect.Api.Dtos;

public sealed class SubmitWordSuggestionRequestDto
{
    [Required]
    [StringLength(200)]
    public string Headword { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Definition { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(320)]
    public string? Email { get; set; }

    [Required]
    [StringLength(4096)]
    public string CaptchaToken { get; set; } = string.Empty;
}
