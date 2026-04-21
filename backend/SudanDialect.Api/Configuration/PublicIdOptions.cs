namespace SudanDialect.Api.Configuration;

public sealed class PublicIdOptions
{
    public const string SectionName = "PublicId";

    public int MinLength { get; set; } = 8;
}
