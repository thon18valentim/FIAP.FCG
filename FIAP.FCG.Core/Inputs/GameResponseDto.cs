namespace FIAP.FCG.Core.Inputs
{
    public sealed record GameResponseDto(
        int Id,
        string Name,
        string Platform,
        string PublisherName,
        string Description,
        double Price,
        DateTime CreatedAtUtc
        );
}
