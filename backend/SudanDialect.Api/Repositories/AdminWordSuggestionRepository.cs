using Microsoft.EntityFrameworkCore;
using SudanDialect.Api.Data;
using SudanDialect.Api.Dtos.Admin;
using SudanDialect.Api.Interfaces.Repositories;

namespace SudanDialect.Api.Repositories;

public sealed class AdminWordSuggestionRepository : IAdminWordSuggestionRepository
{
    private readonly AppDbContext _dbContext;

    public AdminWordSuggestionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<AdminWordSuggestionItemDto> Items, int TotalCount)> GetPagedAsync(
        string? query,
        bool? resolved,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var suggestions = _dbContext.WordSuggestions.AsNoTracking();

        if (resolved.HasValue)
        {
            suggestions = suggestions.Where(item => item.Resolved == resolved.Value);
        }

        var normalizedQuery = query?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedQuery))
        {
            var pattern = $"%{normalizedQuery}%";
            suggestions = suggestions.Where(item =>
                EF.Functions.ILike(item.Headword, pattern)
                || EF.Functions.ILike(item.Definition, pattern)
                || (item.Email != null && EF.Functions.ILike(item.Email, pattern)));
        }

        var totalCount = await suggestions.CountAsync(cancellationToken);

        var sortedSuggestions = sortDescending
            ? suggestions.OrderByDescending(item => item.Timestamp).ThenByDescending(item => item.Id)
            : suggestions.OrderBy(item => item.Timestamp).ThenBy(item => item.Id);

        var skip = (page - 1) * pageSize;
        var items = await sortedSuggestions
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new AdminWordSuggestionItemDto
            {
                Id = item.Id,
                Headword = item.Headword,
                Definition = item.Definition,
                Email = item.Email,
                Resolved = item.Resolved,
                Timestamp = item.Timestamp
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> SetResolvedAsync(int id, bool resolved, CancellationToken cancellationToken = default)
    {
        var suggestion = await _dbContext.WordSuggestions.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (suggestion is null)
        {
            return false;
        }

        if (suggestion.Resolved == resolved)
        {
            return true;
        }

        suggestion.Resolved = resolved;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
