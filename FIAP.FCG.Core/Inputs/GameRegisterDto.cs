
using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.Core.Inputs;

public sealed record class GameRegisterDto
{
    [Required, MinLength(2)]
    public required string Name { get; init; } = default!;
    [Required, MinLength(2)]
    public required string Platform { get; init; } = default!;
    [Required, Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero.")]
    public required double Price { get; init; } = default!;
}
 
