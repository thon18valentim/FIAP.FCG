using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.Core.Inputs
{
    public sealed record GameUpdateDto
    {
        [MinLength(2)]
        public string? Name { get; init; } = default!;

        [MinLength(2)]
        public string? Platform { get; init; } = default!;

        [Required, MinLength(2)]
        public required string PublisherName { get; init; } = default!;

        [Required, MinLength(2), MaxLength(100)]
        public required string Description { get; init; } = default!;

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero.")]
        public double Price { get; init; } = default!;
    }
}
