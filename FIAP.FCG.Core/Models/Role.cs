namespace FIAP.FCG.Core.Models
{

    public class Role : EntityBase
    {
        public required string Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        protected Role()
        {
            
        }
    }
}
