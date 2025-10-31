
using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.Core.Inputs
{
	public sealed record class LoginDto
	{
		[EmailAddress]
		public required string Email { get; set; }
		public required string Password { get; set; }
	}
}
