using ToDo.Data.Entities.Base;

namespace ToDo.Data.Entities
{
    public class Role : BaseEntity
    {
        public Role()
        {
            // Initialize the collection in the constructor
            UserRoles = new HashSet<UserRole>();
        }
        public string Name { get; set; } = string.Empty; // e.g. "Admin", "User"

        // Relationships
        public ICollection<UserRole> UserRoles { get; set; }

    }
}
