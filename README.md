# Moq.EntityFrameworkCore.Helpers
[![Build Status](https://travis-ci.org/MichalJankowskii/Moq.EntityFrameworkCore.Helpers.svg?branch=master)](https://travis-ci.org/MichalJankowskii/Moq.EntityFrameworkCore.Helpers)
[![Downloads](https://img.shields.io/nuget/dt/Moq.EntityFrameworkCore.Helpers.svg)](https://github.com/MichalJankowskii/Moq.EntityFrameworkCore.Helpers)

This library helps you with mocking EntityFramework contexts. Now you will be able to test methods that are using `DbSet<TEntity>` from `DbContext` in effective way.
## Installation - NuGet Packages
```
Install-Package Moq.EntityFrameworkCore.Helpers
```

## Usage
For example we can assume that we have following production code:
```
public class UsersContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
}
```

For mocking Users you need only implement following 3 steps:

1\. Create `DbContext` mock:
```csharp
var userContextMock = new Mock<UsersContext>();
```
2\. Generate your entities:
```csharp
IList<User> users = ...;
```
3\. Setup `DbSet` propery:
```csharp
userContextMock.Setup(x => x.Users).Returns(users);
```

And this is all. You can use your `DbContext` in your tests.

You will find examples of this library in [repository](https://github.com/MichalJankowskii/Moq.EntityFrameworkCore.Helpers/blob/master/src/Moq.EntityFrameworkCore.Helpers.Examples/UsersServiceTest.cs).
