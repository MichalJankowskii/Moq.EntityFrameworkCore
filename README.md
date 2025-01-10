# Moq.EntityFrameworkCore
[![Build Status](https://dev.azure.com/OpenSource-jankowskimichalpl/Moq.EntityFrameworkCore/_apis/build/status/MichalJankowskii.Moq.EntityFrameworkCore?branchName=master)](https://dev.azure.com/OpenSource-jankowskimichalpl/Moq.EntityFrameworkCore/_build/latest?definitionId=1&branchName=master)
[![Downloads](https://img.shields.io/nuget/dt/Moq.EntityFrameworkCore.svg)](https://www.nuget.org/packages/Moq.EntityFrameworkCore/)

This library helps you mocking EntityFramework contexts. Now you will be able to test methods that are using `DbSet<TEntity>` or `DbQuery<TEntity>` from `DbContext` in an effective way.
## Installation - NuGet Packages
```
Install-Package Moq.EntityFrameworkCore
```

## Usage
For example we can assume that we have the following production code:
```
public class UsersContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
}
```

To mock Users and Roles you only need to implement the following 3 steps:

1\. Create `DbContext` mock:
```csharp
var userContextMock = new Mock<UsersContext>();
```
2\. Generate your entities:
```csharp
IList<User> users = ...;
```
3\. Setup `DbSet` or `DbQuery` property:
```csharp
userContextMock.Setup(x => x.Users).ReturnsDbSet(users);
```
or 
```csharp
userContextMock.SetupGet(x => x.Users).ReturnsDbSet(users);
```
or
```csharp
userContextMock.SetupSequence(x => x.Set<User>())
  .ReturnsDbSet(new List<User>())
  .ReturnsDbSet(users);
```



And this is all. You can use your `DbContext` in your tests.

The second option is mocking `DbSet` that is part of the interface:
```csharp
public interface IBlogContext
{
   DbSet<Post> Posts { get; }
}
```

And then use:
```csharp
var posts = new List<Post>();
var contextMock = new Mock<IBlogContext>();
contextMock.Setup(p => p.Posts).ReturnsDbSet(posts);
```

## Using ReturnsDbSetWithGlobalFilter
You can also use `ReturnsDbSetWithGlobalFilter` to set up a `DbSet` with a global filter. For example:
```csharp
var users = new List<User> { new User { IsActive = true }, new User { IsActive = false } };
var userContextMock = new Mock<UsersContext>();
userContextMock.Setup(x => x.Users).ReturnsDbSetWithGlobalFilter(users, u => u.IsActive);
```

You will find examples of this library in the [repository](https://github.com/MichalJankowskii/Moq.EntityFrameworkCore/blob/master/src/Moq.EntityFrameworkCore.Examples/UsersServiceTest.cs).
