namespace Moq.EntityFrameworkCore.Examples.Users
{
    using Microsoft.EntityFrameworkCore;
    using Moq.EntityFrameworkCore.Examples.Users.Entities;

    public class UsersContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
    }
}