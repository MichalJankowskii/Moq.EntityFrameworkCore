﻿namespace Moq.EntityFrameworkCore.Examples
{
    using AutoFixture;
    using Moq.EntityFrameworkCore;
    using Moq.EntityFrameworkCore.Examples.Users;
    using Moq.EntityFrameworkCore.Examples.Users.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class UsersServiceTest
    {
        private static readonly Fixture Fixture = new Fixture();

        [Fact]
        public void Given_ListOfUsersWithOneUserAccountLock_When_CheckingWhoIsLocked_Then_CorrectLockedUserIsReturned()
        {
            // Arrange
            IList<User> users = GenerateNotLockedUsers();
            var lockedUser = Fixture.Build<User>().With(u => u.AccountLocked, true).Create();
            users.Add(lockedUser);

            var userContextMock = new Mock<UsersContext>();
            userContextMock.Setup(x => x.Users).ReturnsDbSet(users);

            var usersService = new UsersService(userContextMock.Object);

            // Act
            var lockedUsers = usersService.GetLockedUsers();

            // Assert
            Assert.Equal(new List<User> { lockedUser }, lockedUsers);
        }

        [Fact]
        public async Task Given_ListOfUsersWithOneUserAccountLock_When_CheckingWhoIsLockedAsync_Then_CorrectLockedUserIsReturned()
        {
            // Arrange
            IList<User> users = GenerateNotLockedUsers();
            var lockedUser = Fixture.Build<User>().With(u => u.AccountLocked, true).Create();
            users.Add(lockedUser);

            var userContextMock = new Mock<UsersContext>();
            userContextMock.Setup(x => x.Users).ReturnsDbSet(users);

            var usersService = new UsersService(userContextMock.Object);

            // Act
            var lockedUsers = await usersService.GetLockedUsersAsync();

            // Assert
            Assert.Equal(new List<User> { lockedUser }, lockedUsers);
        }

        [Fact]
        public void Given_ListOfGroupsWithOneGroupDisabled_When_CheckingWhichOneIsDisabled_Then_CorrectDisabledRoleIsReturned()
        {
            // Arrange
            IList<Role> roles = GenerateEnabledGroups();
            var disabledRole = Fixture.Build<Role>().With(u => u.IsEnabled, false).Create();
            roles.Add(disabledRole);

            var userContextMock = new Mock<UsersContext>();
            userContextMock.Setup(x => x.Roles).ReturnsDbSet(roles);

            var usersService = new UsersService(userContextMock.Object);

            // Act
            var disabledRoles = usersService.GetDisabledRoles();

            // Assert
            Assert.Equal(new List<Role> { disabledRole }, disabledRoles);
        }

        [Fact]
        public async Task Given_ListOfGroupsWithOneGroupDisabled_When_CheckingWhichOneIsDisabledAsync_Then_CorrectDisabledRoleIsReturned()
        {
            // Arrange
            IList<Role> roles = GenerateEnabledGroups();
            var disabledRole = Fixture.Build<Role>().With(u => u.IsEnabled, false).Create();
            roles.Add(disabledRole);

            var userContextMock = new Mock<UsersContext>();
            userContextMock.Setup(x => x.Roles).ReturnsDbSet(roles);

            var usersService = new UsersService(userContextMock.Object);

            // Act
            var disabledRoles = await usersService.GetDisabledRolesAsync();

            // Assert
            Assert.Equal(new List<Role> { disabledRole }, disabledRoles);
        }

        private static IList<User> GenerateNotLockedUsers()
        {
            IList<User> users = new List<User>
            {
                Fixture.Build<User>().With(u => u.AccountLocked, false).Create(),
                Fixture.Build<User>().With(u => u.AccountLocked, false).Create()
            };

            return users;
        }

        private static IList<Role> GenerateEnabledGroups()
        {
            IList<Role> users = new List<Role>
            {
                Fixture.Build<Role>().With(u => u.IsEnabled, true).Create(),
                Fixture.Build<Role>().With(u => u.IsEnabled, true).Create()
            };

            return users;
        }
    }
}