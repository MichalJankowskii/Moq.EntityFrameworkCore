# Moq.EntityFrameworkCore
[![Build Status](https://travis-ci.org/MichalJankowskii/Moq.EntityFrameworkCore.svg?branch=master)](https://travis-ci.org/MichalJankowskii/Moq.EntityFrameworkCore)
[![Downloads](https://img.shields.io/nuget/dt/Moq.EntityFrameworkCore.svg)](https://www.nuget.org/packages/Moq.EntityFrameworkCore/)

This library helps you with mocking EntityFramework contexts. Now you will be able to test methods that are using `DbSet<TEntity>` or `DbQuery<TEntity>` from `DbContext` in an effective way.
## Installation - NuGet Packages
```
Install-Package Moq.EntityFrameworkCore
```

## Usage
For example we can assume that we have following production code:
```
public class UsersContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }

    public virtual DbQuery<Role> Roles { get; set; }
}
```

For mocking Users you need only to implement following 3 steps:

1\. Create `DbContext` mock:
```csharp
var userContextMock = new Mock<UsersContext>();
```
2\. Generate your entities:
```csharp
IList<User> users = ...;
IList<Roles> roles = ...;
```
3\. Setup `DbSet` or `DbQuery` property:
```csharp
userContextMock.Setup(x => x.Users).ReturnsDbSet(users);
userContextMock.Setup(x => x.Roles).ReturnsDbQuery(roles);
```

And this is all. You can use your `DbContext` in your tests.

You will find examples of this library in [repository](https://github.com/MichalJankowskii/Moq.EntityFrameworkCore/blob/master/src/Moq.EntityFrameworkCore.Examples/UsersServiceTest.cs).
