
using ToDo.Data.Entities.Base;

namespace ToDo.Data.Entities
{
    public class ToDos : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; }

        public DateOnly? ToDoAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
