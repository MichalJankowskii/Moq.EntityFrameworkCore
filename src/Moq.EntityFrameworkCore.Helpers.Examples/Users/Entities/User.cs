namespace Moq.EntityFrameworkCore.Helpers.Examples.Users.Entities
{
    using System.Collections.Generic;

    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public bool AccountLocked { get; set; }

        public virtual List<Role> Roles { get; set; }
    }
}
