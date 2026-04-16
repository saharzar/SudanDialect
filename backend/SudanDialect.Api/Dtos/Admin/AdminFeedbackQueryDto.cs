using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminFeedbackQueryDto
{
    [Range(1, int.MaxValue)]
    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 20;

    [FromQuery(Name = "resolved")]
    public bool? Resolved { get; set; }

    [Range(1, int.MaxValue)]
    [FromQuery(Name = "wordId")]
    public int? WordId { get; set; }

    [StringLength(10)]
    [FromQuery(Name = "sortDirection")]
    public string SortDirection { get; set; } = "desc";
}
