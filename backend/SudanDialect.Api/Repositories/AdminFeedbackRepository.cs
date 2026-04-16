using Microsoft.EntityFrameworkCore;
using SudanDialect.Api.Data;
using SudanDialect.Api.Dtos.Admin;
using SudanDialect.Api.Interfaces.Repositories;

namespace SudanDialect.Api.Repositories;

public sealed class AdminFeedbackRepository : IAdminFeedbackRepository
{
    private readonly AppDbContext _dbContext;

    public AdminFeedbackRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<AdminFeedbackItemDto> Items, int TotalCount)> GetPagedAsync(
        bool? resolved,
        int? wordId,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Feedback.AsNoTracking();

        if (resolved.HasValue)
        {
            query = query.Where(item => item.Resolved == resolved.Value);
        }

        if (wordId.HasValue)
        {
            query = query.Where(item => item.WordId == wordId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var sortedQuery = sortDescending
            ? query.OrderByDescending(item => item.Timestamp).ThenByDescending(item => item.Id)
            : query.OrderBy(item => item.Timestamp).ThenBy(item => item.Id);

        var skip = (page - 1) * pageSize;
        var items = await sortedQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new AdminFeedbackItemDto
            {
                Id = item.Id,
                WordId = item.WordId,
                WordHeadword = item.Word.Headword,
                FeedbackText = item.FeedbackText,
                Timestamp = item.Timestamp,
                Resolved = item.Resolved
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> SetResolvedAsync(int id, bool resolved, CancellationToken cancellationToken = default)
    {
        var feedback = await _dbContext.Feedback.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (feedback is null)
        {
            return false;
        }

        if (feedback.Resolved == resolved)
        {
            return true;
        }

        feedback.Resolved = resolved;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
