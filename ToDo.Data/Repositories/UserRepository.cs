using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Data.Entities;

namespace ToDo.Data.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<User?> GetByUsernameAsync(string username)
        //{
        //    return await _context.Users.Include(u => u.Role)
        //        .FirstOrDefaultAsync(u => u.Username == username);
        //}

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
