using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.FCG.Core.Inputs
{
    public sealed record UserResponseDto(
        int Id,
        string Name,
        string Email,
        string Address,
        string Cpf,
        DateTime CreatedAtUtc
    );

}
