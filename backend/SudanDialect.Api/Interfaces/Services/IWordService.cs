using SudanDialect.Api.Dtos;

namespace SudanDialect.Api.Interfaces.Services;

public interface IWordService
{
    Task<WordSearchResultDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<WordPageDto> BrowseByLetterAsync(
        string? rawLetter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WordSearchResultDto>> SearchAsync(string? rawQuery, CancellationToken cancellationToken = default);

    Task<bool> SubmitFeedbackAsync(
        int wordId,
        string? feedbackText,
        string? captchaToken,
        string? remoteIp,
        CancellationToken cancellationToken = default);

    Task<bool> SubmitSuggestionAsync(
        string? headword,
        string? definition,
        string? email,
        string? captchaToken,
        string? remoteIp,
        CancellationToken cancellationToken = default);
}
