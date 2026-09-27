using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Domain;

namespace ToDoList.Api.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options) 
        {
        }

        public DbSet<Todo> Todos { get; set; }
    }
}
