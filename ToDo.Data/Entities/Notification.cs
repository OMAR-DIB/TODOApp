using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Data.Entities.Base;

namespace ToDo.Data.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }   
        public User? User { get; set; }

        public int ToDoId { get; set; }   
        public ToDos? ToDo { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
    }
}
