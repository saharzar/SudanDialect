namespace SudanDialect.Api.Utilities;

public static class AdminWordEditActionTypes
{
    public const string Update = "update";
    public const string Deactivate = "deactivate";
    public const string Reactivate = "reactivate";

    public static readonly IReadOnlySet<string> Allowed =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { Update, Deactivate, Reactivate };

    public static string Resolve(bool oldIsActive, bool newIsActive)
    {
        if (oldIsActive && !newIsActive)
        {
            return Deactivate;
        }

        if (!oldIsActive && newIsActive)
        {
            return Reactivate;
        }

        return Update;
    }
}
