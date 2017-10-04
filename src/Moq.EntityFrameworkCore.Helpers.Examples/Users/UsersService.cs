namespace Moq.EntityFrameworkCore.Helpers.Examples.Users
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public class UsersService
    {
        private readonly UsersContext usersContext;

        public UsersService(UsersContext usersContext)
        {
            this.usersContext = usersContext;
        }

        public IList<User> GetLockedUsers()
        {
            return this.usersContext.Users.Where(x => x.AccountLocked).ToList();
        }

        public async Task<IList<User>> GetLockedUsersAsync()
        {
            return await this.usersContext.Users.Where(x => x.AccountLocked).ToListAsync();
        }
    }
}