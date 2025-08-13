using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Models.Forum;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Forum;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace CarSellingPlatform.Services.Core.Tests;
[TestFixture]
public class ForumServiceTests
{
    private Mock<IRepository<Post, Guid>> _mockForumRepository;
    private Mock<IRepository<Comment, Guid>> _mockCommentRepository;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private IForumService _forumService;

    [SetUp]
    public void SetUp()
    {
        _mockForumRepository = new Mock<IRepository<Post, Guid>>();
        _mockCommentRepository = new Mock<IRepository<Comment, Guid>>();
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        // Initialize ForumService with mocked dependencies
        _forumService = new ForumService(
            _mockForumRepository.Object,
            _mockCommentRepository.Object,
            _mockUserManager.Object
        );
    }
    
    [Test]
    public async Task ListPagedAsync_ShouldReturnPostsSortedByLatest_WhenSortByIsLatest()
    {
        
        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), Title = "Old Post", CreatedAt = DateTime.UtcNow.AddDays(-2), Comments = new List<Comment>() },
            new Post { Id = Guid.NewGuid(), Title = "Newest Post", CreatedAt = DateTime.UtcNow, Comments = new List<Comment>() },
            new Post { Id = Guid.NewGuid(), Title = "Older Post", CreatedAt = DateTime.UtcNow.AddDays(-1), Comments = new List<Comment>() }
        };
        var mockQueryable = posts.BuildMock();
        _mockForumRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
        
        const int pageNumber = 1;
        const int pageSize = 2;
        const string sortBy = "latest";
        
        
        var result = await _forumService.ListPagedAsync(null, pageNumber, pageSize, sortBy);
        
        
        Assert.IsNotNull(result);
        Assert.AreEqual(pageSize, result.Items.Count());
        Assert.AreEqual(2, result.TotalPages); // Ceiling(3 total / 2 per page) = 2
        Assert.AreEqual("Newest Post", result.Items.First().Title);
        Assert.AreEqual("Older Post", result.Items.Last().Title);
        Assert.AreEqual(sortBy, result.SortBy);
    }
    [Test]
    public async Task ListPagedAsync_ShouldReturnPostsSortedByComments_WhenSortByIsComments()
    {
        
        var posts = new List<Post>
        {
            new Post { Id = Guid.NewGuid(), Title = "Fewest Comments", CreatedAt = DateTime.UtcNow, Comments = new List<Comment>() },
            new Post { Id = Guid.NewGuid(), Title = "Most Comments", CreatedAt = DateTime.UtcNow, Comments = new List<Comment> { new(), new(), new() } },
            new Post { Id = Guid.NewGuid(), Title = "Some Comments", CreatedAt = DateTime.UtcNow, Comments = new List<Comment> { new() } }
        };
        var mockQueryable = posts.BuildMock();
        _mockForumRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        const int pageNumber = 1;
        const int pageSize = 3;
        const string sortBy = "comments";

        
        var result = await _forumService.ListPagedAsync(null, pageNumber, pageSize, sortBy);
        
        
        Assert.IsNotNull(result);
        Assert.AreEqual(pageSize, result.Items.Count());
        Assert.AreEqual(1, result.TotalPages); 
        Assert.AreEqual("Most Comments", result.Items.First().Title);
        Assert.AreEqual("Fewest Comments", result.Items.Last().Title);
        Assert.AreEqual(sortBy, result.SortBy);
    }
    [Test]
    public async Task AddPostAsync_WhenUserExists_ShouldAddPostAndReturnTrue()
    {
        
        const string userId = "existing-user-id";
        var user = new ApplicationUser { Id = userId };
        var inputModel = new AddPostInputModel { Title = "Test Title", Text = "Test text content." };

        _mockUserManager.Setup(um => um.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _mockForumRepository.Setup(r => r.AddAsync(It.IsAny<Post>()))
            .Returns(Task.CompletedTask);
    
        _mockForumRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        
        var result = await _forumService.AddPostAsync(userId, inputModel);

        
        Assert.IsTrue(result);
        _mockForumRepository.Verify(r => r.AddAsync(It.Is<Post>(p => p.Title == inputModel.Title && p.AuthorId == userId)), Times.Once);
        _mockForumRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    [Test]
    public async Task AddPostAsync_WhenUserDoesNotExist_ShouldNotAddPostAndReturnFalse()
    {
        
        const string userId = "non-existent-user-id";
        var inputModel = new AddPostInputModel { Title = "Test Title", Text = "Test text content." };

        _mockUserManager.Setup(um => um.FindByIdAsync(userId))
            .ReturnsAsync((ApplicationUser)null);

        
        var result = await _forumService.AddPostAsync(userId, inputModel);

        
        Assert.IsFalse(result);
        _mockForumRepository.Verify(r => r.AddAsync(It.IsAny<Post>()), Times.Never);
        _mockForumRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task AddCommentAsync_WhenUserExists_ShouldAddCommentAndReturnFalseDueToBug()
    {
        const string userId = "existing-user-id";
        var postId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var inputModel = new AddCommentInputModel { NewCommentText = "This is a test comment." };

        _mockUserManager.Setup(um => um.FindByIdAsync(userId))
            .ReturnsAsync(user);
    
        _mockCommentRepository.Setup(r => r.AddAsync(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask);
    
        _mockCommentRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var result = await _forumService.AddCommentAsync(userId, inputModel, postId);

        Assert.IsTrue(result); 
        _mockCommentRepository.Verify(r => r.AddAsync(It.Is<Comment>(c => c.Text == inputModel.NewCommentText && c.PostId == postId)), Times.Once);
        _mockCommentRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    [Test]
    public async Task AddCommentAsync_WhenUserDoesNotExist_ShouldNotAddCommentAndReturnFalse()
    {
        const string userId = "non-existent-user-id";
        var postId = Guid.NewGuid();
        var inputModel = new AddCommentInputModel { NewCommentText = "This comment should not be added." };

        _mockUserManager.Setup(um => um.FindByIdAsync(userId))
            .ReturnsAsync((ApplicationUser)null);

        var result = await _forumService.AddCommentAsync(userId, inputModel, postId);

        Assert.IsFalse(result);
        _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        _mockCommentRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task GetDetailsPostAsync_WhenPostExistsAndUserIsOwner_ShouldReturnViewModelWithIsOwnerTrue()
    {
        
        const string userId = "post-owner-id";
        var postId = Guid.NewGuid();
        var author = new ApplicationUser { Id = userId, UserName = "Owner" };

        var posts = new List<Post>
        {
            new Post
            {
                Id = postId,
                Title = "Test Post",
                AuthorId = userId,
                Author = author,
                Comments = new List<Comment>()
            }
        };
    
        var mockQueryable = posts.BuildMock();
        _mockForumRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
    
        
        var result = await _forumService.GetDetailsPostAsync(postId, userId);

        
        Assert.IsNotNull(result);
        Assert.AreEqual(postId, result.Post.Id);
        Assert.AreEqual("Test Post", result.Post.Title);
        Assert.AreEqual(author.UserName, result.Author.UserName);
        Assert.IsTrue(result.IsOwner);
    }
    [Test]
    public async Task GetDetailsPostAsync_WhenPostDoesNotExist_ShouldReturnNull()
    {
        
        var nonExistentPostId = Guid.NewGuid();
        var posts = new List<Post>();
        var mockQueryable = posts.BuildMock();
        _mockForumRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
        var result = await _forumService.GetDetailsPostAsync(nonExistentPostId, "any-user-id");
        Assert.IsNull(result);
    }

}