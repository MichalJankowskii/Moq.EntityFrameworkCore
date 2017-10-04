namespace Moq.EntityFrameworkCore.Helpers.Examples.Users
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public class UsersContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
    }
}