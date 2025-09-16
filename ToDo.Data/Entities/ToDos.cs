
using ToDo.Data.Entities.Base;
using ToDo.Data.Entities.@enum;

namespace ToDo.Data.Entities
{
    public class ToDos : BaseEntity
    {
        public ToDos() {
            SubTasks = new HashSet<SubTask>();
            Notifications = new HashSet<Notification>();
        }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatuss Status { get; set; } = TaskStatuss.ToDo;

        public DateOnly? ToDoAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<SubTask> SubTasks { get; set; } 
        public ICollection<Notification> Notifications { get; set; } 
    }
}
