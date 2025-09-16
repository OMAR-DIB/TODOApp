
using ToDo.Data.Entities.Base;
using ToDo.Data.Entities.@enum;

namespace ToDo.Data.Entities
{
    public class SubTask : BaseEntity
    {
            public int ToDoId { get; set; }   // FK → ToDo
            public ToDos ToDo { get; set; }   // Navigation

            public string Title { get; set; } = string.Empty;

            public TaskStatuss Status { get; set; } = TaskStatuss.ToDo; // use enum

    }
}
