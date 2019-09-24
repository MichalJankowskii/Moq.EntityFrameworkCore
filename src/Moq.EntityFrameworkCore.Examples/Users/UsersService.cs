using System.Threading;

namespace Moq.EntityFrameworkCore.Examples.Users
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq.EntityFrameworkCore.Examples.Users.Entities;

    public class UsersService
    {
        private readonly UsersContext usersContext;

        public UsersService(UsersContext usersContext)
        {
            this.usersContext = usersContext;
        }


        public async Task<User> GetOne(int userId)
        {
            return await this.usersContext.Users
                .Include(u => u.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, CancellationToken.None);
        }
        public IList<User> GetLockedUsers()
        {
            return this.usersContext.Users.Where(x => x.AccountLocked).ToList();
        }

        public async Task<IList<User>> GetLockedUsersAsync()
        {
            return await this.usersContext.Users.Where(x => x.AccountLocked).ToListAsync();
        }

        public IList<Role> GetDisabledRoles()
        {
            return this.usersContext.Roles.Where(x => !x.IsEnabled).ToList();
        }

        public async Task<IList<Role>> GetDisabledRolesAsync()
        {
            return await this.usersContext.Roles.Where(x => !x.IsEnabled).ToListAsync();
        }
    }
}