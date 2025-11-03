
namespace FIAP.FCG.Core.Models
{
	public class User : EntityBase
	{
		public required string Name { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string Cpf { get; set; }
		public required string Address { get; set; }
		public required bool IsAdmin { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
