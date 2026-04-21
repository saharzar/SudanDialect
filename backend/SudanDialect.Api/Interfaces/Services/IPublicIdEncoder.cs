namespace SudanDialect.Api.Interfaces.Services;

public interface IPublicIdEncoder
{
    string EncodeWordId(int id);

    bool TryDecodeWordId(string encodedId, out int id);
}
