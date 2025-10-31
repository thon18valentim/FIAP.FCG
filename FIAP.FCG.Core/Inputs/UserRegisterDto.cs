using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.Core.Inputs
{
    public sealed record class UserRegisterDto
    {
        [Required, MinLength(2)]
        public string Name { get; init; } = default!;
    
        [Required, EmailAddress]
        public string Email { get; init; } = default!;
    
        [Required, MinLength(3)]
        public string Address { get; init; } = default!;
    
        [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 dígitos numéricos.")]
        public string Cpf { get; init; } = default!;
    
        [Required, RegularExpression(@"^(?=.*[A-Z])(?=(?:.*\d){3,})(?=.*[^\w\s])\S{8,}$",
                 ErrorMessage = "Senha deve ter ao menos 8 caracteres, 3 dígitos, 1 maiúscula e 1 caractere especial.")]
        public string Password { get; init; } = default!;
    }
}
