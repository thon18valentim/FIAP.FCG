namespace FIAP.FCG.Core.Inputs
{
    public sealed record GameResponseDto(
        int Id,
        string Name,
        string Platform,
        double Price,
        DateTime CreatedAtUtc
        );
}
