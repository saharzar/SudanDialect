using System.ComponentModel.DataAnnotations;

namespace SudanDialect.Api.Dtos;

public sealed class SubmitWordFeedbackRequestDto
{
    [Required]
    [StringLength(2000)]
    public string FeedbackText { get; set; } = string.Empty;

    [Required]
    [StringLength(4096)]
    public string CaptchaToken { get; set; } = string.Empty;
}
