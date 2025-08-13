using System.Linq.Expressions;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Models.Forum;
using CarSellingPlatform.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace CarSellingPlatform.Services.Core.Tests;
[TestFixture]
public class ForumManagerServiceTests
{
    private Mock<IRepository<Post, Guid>> _mockPostRepository;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private IForumManagerService _forumManagerService;

    [SetUp]
    public void SetUp()
    {
        _mockPostRepository = new Mock<IRepository<Post, Guid>>();
        
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _forumManagerService = new ForumManagerService(
            _mockPostRepository.Object,
            _mockUserManager.Object
        );
    }
    [Test]
    public async Task ListPagedAsync_WhenPostsExist_ShouldReturnPaginatedViewModel()
    {
        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), Title = "Post 1", Author = new ApplicationUser { FirstName = "John", LastName = "Doe" } },
            new Post { Id = Guid.NewGuid(), Title = "Post 2", Author = new ApplicationUser { FirstName = "Jane", LastName = "Smith" } },
            new Post { Id = Guid.NewGuid(), Title = "Post 3", Author = new ApplicationUser { FirstName = "Peter", LastName = "Jones" } }
        };

        var mockQueryable = posts.BuildMock();
        _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _forumManagerService.ListPagedAsync(null, 2, 2);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count());
        Assert.AreEqual(2, result.TotalPages);
        Assert.AreEqual("Post 3", result.Items.First().Title);
        Assert.AreEqual("Peter Jones", result.Items.First().AuthorName);
    }
    
    [Test]
    public async Task ListPagedAsync_WhenNoPostsExist_ShouldReturnEmptyViewModel()
    {
        var posts = new List<Post>();
        var mockQueryable = posts.BuildMock();
        _mockPostRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _forumManagerService.ListPagedAsync(null, 1, 10);

        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Items);
        Assert.AreEqual(0, result.TotalPages);
    }
    [Test]
    public async Task HardDeletePost_WhenUserAndPostExist_ShouldDeleteAndReturnTrue()
    {
        var userId = "user-id";
        var postId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var post = new Post { Id = postId };

        _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockPostRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Post, bool>>>()))
            .ReturnsAsync(post);
        
        var result = await _forumManagerService.HardDeleteDealership(postId, userId);

        Assert.IsTrue(result);
        _mockPostRepository.Verify(r => r.HardDelete(post), Times.Once);
        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task HardDeletePost_WhenPostDoesNotExist_ShouldNotDeleteAndReturnFalse()
    {
        var userId = "user-id";
        var postId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };

        _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockPostRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Post, bool>>>()))
            .ReturnsAsync((Post)null);

        var result = await _forumManagerService.HardDeleteDealership(postId, userId);

        Assert.IsFalse(result);
        _mockPostRepository.Verify(r => r.HardDelete(It.IsAny<Post>()), Times.Never);
        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

}