namespace Moq.EntityFrameworkCore.Examples
{
    using AutoFixture;
    using Moq.EntityFrameworkCore;
    using Moq.EntityFrameworkCore.Examples.Users;
    using Moq.EntityFrameworkCore.Examples.Users.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;
    using System.Linq;
    using System.Threading;
    using Microsoft.EntityFrameworkCore;

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
        public void Given_ListOfUsersWithOneUserAccountLockAndMockingWithSetupGet_When_CheckingWhoIsLocked_Then_CorrectLockedUserIsReturned()
        {
            // Arrange
            IList<User> users = GenerateNotLockedUsers();
            var lockedUser = Fixture.Build<User>().With(u => u.AccountLocked, true).Create();
            users.Add(lockedUser);

            var userContextMock = new Mock<UsersContext>();
            userContextMock.SetupGet(x => x.Users).ReturnsDbSet(users);

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

        [Fact]
        public async Task Given_ListOfUser_When_FindOneUserAsync_Then_CorrectUserIsReturned()
        {
            var users = GenerateNotLockedUsers();
            var userContextMock = new Mock<UsersContext>();
            userContextMock.Setup(x => x.Set<User>()).ReturnsDbSet(users);

            var usersService = new UsersService(userContextMock.Object);
            var user = users.FirstOrDefault();

            //Act
            var userToAssert = await usersService.FindOneUserAsync(x => x.Id == user.Id);

            //Assert
            Assert.Equal(userToAssert, user);
        }

        [Fact]
        public async Task Given_Two_ListOfUser_Then_CorrectListIsReturned_InSequence()
        {
            var users = GenerateNotLockedUsers();
            var userContextMock = new Mock<UsersContext>();
            userContextMock.SetupSequence(x => x.Set<User>())
                .ReturnsDbSet(new List<User>())
                .ReturnsDbSet(users);

            var usersService = new UsersService(userContextMock.Object);

            var user = users.FirstOrDefault();

            //Act
            var userToAssertWhenFirstCall = await usersService.FindOneUserAsync(x => x.Id == user.Id);
            var userToAssertWhenSecondCall = await usersService.FindOneUserAsync(x => x.Id == user.Id);

            //Assert
            Assert.Null(userToAssertWhenFirstCall);
            Assert.Equal(userToAssertWhenSecondCall, user);
        }

        [Fact]
        public async Task Given_ListOfUser_When_AddingUser_Then_CorrectMethodsExecuted()
        {
            // Arrange
            var user = Fixture.Build<User>().Create();
            var usersDb = new Mock<DbSet<User>>();
            var usersContextMock = new Mock<UsersContext>();
            usersContextMock.Setup(x => x.Users).Returns(usersDb.Object);
            var usersService = new UsersService(usersContextMock.Object);

            // Act
            await usersService.AddUser(user);

            // Assert
            usersDb.Verify(x => x.AddAsync(It.Is<User>(y => y == user), CancellationToken.None), Times.Once);
            usersContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Given_UsersWithGlobalFilter_When_GetAllUsersAsync_Then_OnlyFilteredUsersAreReturned()
        {
            // Arrange
            var userJohn = new User { Id = 1, Name = "John" };
            var userJack = new User { Id = 3, Name = "Jack" };

            var usersContextMock = new Mock<UsersContext>();
            usersContextMock.Setup(x => x.Users)
                .ReturnsDbSetWithGlobalFilter(
                    new List<User> { userJohn, userJack },
                    user => user.Name == "John"
                );

            var usersService = new UsersService(usersContextMock.Object);

            // Act
            var result = await usersService.GetAllUsersAsync(CancellationToken.None); // should exclude userJack

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
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