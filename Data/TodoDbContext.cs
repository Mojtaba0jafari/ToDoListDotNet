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
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Todo>(entity =>
            {
                entity.Property(todo => todo.Title)
                    .HasMaxLength(TodoConstraints.TitleMaxLength);
                entity.Property(todo => todo.Description)
                    .HasMaxLength(TodoConstraints.DescriptionMaxLength);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(user => user.Email)
                    .IsUnique();
            });

            modelBuilder.Entity<User>()
                .HasMany(user => user.Todos)
                .WithOne(todo => todo.User)
                .HasForeignKey(todo => todo.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
