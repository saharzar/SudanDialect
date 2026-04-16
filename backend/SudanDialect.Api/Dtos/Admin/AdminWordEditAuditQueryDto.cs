using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SudanDialect.Api.Dtos.Admin;

public sealed class AdminWordEditAuditQueryDto
{
    [Range(1, int.MaxValue)]
    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 20;

    [StringLength(32)]
    [FromQuery(Name = "actionType")]
    public string? ActionType { get; set; }

    [StringLength(10)]
    [FromQuery(Name = "sortDirection")]
    public string SortDirection { get; set; } = "desc";
}
