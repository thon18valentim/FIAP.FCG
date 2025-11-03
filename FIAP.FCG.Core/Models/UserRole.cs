namespace FIAP.FCG.Core.Models;

public class UserRole
{
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!; // virtual for lazy loading

    public int RoleId { get; set; }
    public virtual Role Role { get; set; } = null!; // virtual for lazy loading

    protected UserRole()
    {

    }
}