using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Repositories;
using UBB_SE_2026_Jobs.Library.Repositories.Chats;
using UBB_SE_2026_Jobs.Library.Repositories.Messages;
using UBB_SE_2026_Jobs.Library.Services.ChatService;
using UBB_SE_2026_Jobs.Library.Services.FileStorage;
using UBB_SE_2026_Jobs.Library.Services.PussyCatsCompanyService;
using UBB_SE_2026_Jobs.Library.Services.Users;
using Xunit;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class ChatServiceTests
{
    private readonly IChatRepository chatRepository = Substitute.For<IChatRepository>();
    private readonly IMessageRepository messageRepository = Substitute.For<IMessageRepository>();
    private readonly IUserService userService = Substitute.For<IUserService>();
    private readonly IPussyCatsCompanyService companyService = Substitute.For<IPussyCatsCompanyService>();
    private readonly ILocalFileStorageService fileStorage = Substitute.For<ILocalFileStorageService>();
    private readonly IRecruiterRepository recruiterRepository = Substitute.For<IRecruiterRepository>();
    private readonly ChatService chatService;

    public ChatServiceTests()
    {
        chatService = new(chatRepository, messageRepository, userService, companyService, fileStorage, recruiterRepository);
    }


    [Fact]
    public async Task FindOrCreateUserChatAsync_CandidateTriesToChatWithRecruiter_Throws()
    {
        recruiterRepository.GetCompanyIdForUserAsync(1, Arg.Any<CancellationToken>()).Returns((int?)null);
        recruiterRepository.GetCompanyIdForUserAsync(2, Arg.Any<CancellationToken>()).Returns((int?)10);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => chatService.FindOrCreateUserChatAsync(userId: 1, secondUserId: 2));
    }

    [Fact]
    public async Task FindOrCreateUserChatAsync_RecruiterTriesToChatWithCandidateAsync_Throws()
    {
        recruiterRepository.GetCompanyIdForUserAsync(1, Arg.Any<CancellationToken>()).Returns((int?)10);
        recruiterRepository.GetCompanyIdForUserAsync(2, Arg.Any<CancellationToken>()).Returns((int?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => chatService.FindOrCreateUserChatAsync(userId: 1, secondUserId: 2));
    }

    [Fact]
    public async Task FindOrCreateUserChatAsync_RecruitersChatAcrossCompanies_Throws()
    {
        recruiterRepository.GetCompanyIdForUserAsync(1, Arg.Any<CancellationToken>()).Returns((int?)10);
        recruiterRepository.GetCompanyIdForUserAsync(2, Arg.Any<CancellationToken>()).Returns((int?)99);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => chatService.FindOrCreateUserChatAsync(userId: 1, secondUserId: 2));
    }

    [Fact]
    public async Task FindOrCreateUserChatAsync_RecruitersFromSameCompany_CreatesChat()
    {
        recruiterRepository.GetCompanyIdForUserAsync(1, Arg.Any<CancellationToken>()).Returns((int?)10);
        recruiterRepository.GetCompanyIdForUserAsync(2, Arg.Any<CancellationToken>()).Returns((int?)10);

        var user1 = new User { UserId = 1 };
        var user2 = new User { UserId = 2 };
        var expectedChat = new Chat();

        chatRepository.FindUserUserChatAsync(1, 2, Arg.Any<CancellationToken>()).Returns((Chat?)null);
        chatRepository.AddAsync(Arg.Any<Chat>(), Arg.Any<CancellationToken>()).Returns(expectedChat);
        userService.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<User>>(new List<User> { user1, user2 }));

        var result = await chatService.FindOrCreateUserChatAsync(userId: 1, secondUserId: 2);

        Assert.Same(expectedChat, result);
    }

    [Fact]
    public async Task FindOrCreateUserChatAsync_TwoCandidates_CreatesChat()
    {
        recruiterRepository.GetCompanyIdForUserAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((int?)null);

        var user1 = new User { UserId = 1 };
        var user2 = new User { UserId = 2 };
        var expectedChat = new Chat();

        chatRepository.FindUserUserChatAsync(1, 2, Arg.Any<CancellationToken>()).Returns((Chat?)null);
        chatRepository.AddAsync(Arg.Any<Chat>(), Arg.Any<CancellationToken>()).Returns(expectedChat);
        userService.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<User>>(new List<User> { user1, user2 }));

            Assert.Equal(existingChat.ChatId, returnedChat!.ChatId);
            Assert.Null(returnedChat!.DeletedAtByUser);
            Assert.Null(returnedChat!.DeletedAtBySecondParty);
        }

        [Fact]
        public async Task FindOrCreateUserChatAsync_CandidateAndRecruiter_ThrowsInvalidOperationException()
        {
            recruiterRepository.GetCompanyIdForUserAsync(1, Arg.Any<CancellationToken>()).Returns((int?)null);
            recruiterRepository.GetCompanyIdForUserAsync(2, Arg.Any<CancellationToken>()).Returns(10);

            await Assert.ThrowsAsync<InvalidOperationException>(() => chatService.FindOrCreateUserChatAsync(1, 2));
        }

        [Fact]
        public async Task FindOrCreateUserChatAsync_RecruitersFromDifferentCompanies_ThrowsInvalidOperationException()
        {
            recruiterRepository.GetCompanyIdForUserAsync(1, Arg.Any<CancellationToken>()).Returns(10);
            recruiterRepository.GetCompanyIdForUserAsync(2, Arg.Any<CancellationToken>()).Returns(20);

            await Assert.ThrowsAsync<InvalidOperationException>(() => chatService.FindOrCreateUserChatAsync(1, 2));
        }

        [Fact]
        public async Task FindOrCreateUserChatAsync_RecruitersFromSameCompany_CreatesAndReturnsNewChat()
        {
            var user1 = new User { UserId = 1 };
            var user2 = new User { UserId = 2 };
            var newChat = new Chat();

            recruiterRepository.GetCompanyIdForUserAsync(1, Arg.Any<CancellationToken>()).Returns(10);
            recruiterRepository.GetCompanyIdForUserAsync(2, Arg.Any<CancellationToken>()).Returns(10);
            chatRepository.FindUserUserChatAsync(1, 2, Arg.Any<CancellationToken>())
                .Returns((Chat?)null);
            chatRepository.AddAsync(Arg.Any<Chat>(), Arg.Any<CancellationToken>())
                .Returns(newChat);
            userService.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<User>>(new List<User> { user1, user2 }));

            var createdChat = await chatService.FindOrCreateUserChatAsync(1, 2);

            Assert.Same(newChat, createdChat);
        }

        [Fact]
        public async Task FindOrCreateUserChatAsync_RecruiterAndCandidate_ThrowsInvalidOperationException()
        {
            recruiterRepository.GetCompanyIdForUserAsync(1, Arg.Any<CancellationToken>()).Returns(10);
            recruiterRepository.GetCompanyIdForUserAsync(2, Arg.Any<CancellationToken>()).Returns((int?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => chatService.FindOrCreateUserChatAsync(1, 2));
        }
        #endregion


    [Fact]
    public async Task GetMessagesAsync_MessagesBeforeDeletionTimestamp_AreNotVisible()
    {
        var deletedAt = new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var callerId = 1;
        var chat = new Chat { User = new User { UserId = callerId }, DeletedAtByUser = deletedAt };

        var beforeDeletion = new Message { Timestamp = deletedAt.AddSeconds(-1), Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } };
        var exactlyAtDeletion = new Message { Timestamp = deletedAt, Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } };
        var afterDeletion = new Message { Timestamp = deletedAt.AddSeconds(1), Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } };

        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);
        messageRepository.GetForChatAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Message>>(new List<Message> { beforeDeletion, exactlyAtDeletion, afterDeletion }));

        var result = await chatService.GetMessagesAsync(chatId: 1, callerId);

        Assert.Single(result);
        Assert.Equal(afterDeletion.Timestamp, result[0].Timestamp);
    }

    [Fact]
    public async Task GetMessagesAsync_NoDeletionTimestamp_AllMessagesAreVisible()
    {
        var callerId = 1;
        var chat = new Chat { User = new User { UserId = callerId }, DeletedAtByUser = null };
        var messages = new List<Message>
        {
            new() { Timestamp = DateTime.UtcNow.AddHours(-2), Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } },
            new() { Timestamp = DateTime.UtcNow.AddHours(-1), Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } },
        };

        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);
        messageRepository.GetForChatAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Message>>(messages));

        var result = await chatService.GetMessagesAsync(chatId: 1, callerId);

        Assert.Equal(2, result.Count);
    }


    [Fact]
    public async Task GetMessagesAsync_OwnMessage_ShowsReadReceiptAndMeLabel()
    {
        var callerId = 1;
        var chat = new Chat { User = new User { UserId = callerId } };
        var ownMessage = new Message { Sender = new MessageSender { SenderId = callerId }, Chat = new Chat { ChatId = 1 }, Timestamp = DateTime.UtcNow };

        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);
        messageRepository.GetForChatAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Message>>(new List<Message> { ownMessage }));

        var result = await chatService.GetMessagesAsync(chatId: 1, callerId);

        Assert.True(result[0].ShowReadReceipt);
        Assert.Equal("Me", result[0].SenderInitials);
    }

    [Fact]
    public async Task GetMessagesAsync_OtherPartyMessage_HidesReadReceiptAndShowsThemLabel()
    {
        var callerId = 1;
        var chat = new Chat { User = new User { UserId = callerId } };
        var otherMessage = new Message { Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 }, Timestamp = DateTime.UtcNow };

        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);
        messageRepository.GetForChatAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Message>>(new List<Message> { otherMessage }));

        var result = await chatService.GetMessagesAsync(chatId: 1, callerId);

        Assert.False(result[0].ShowReadReceipt);
        Assert.Equal("Them", result[0].SenderInitials);
    }


    [Fact]
    public async Task SendMessageAsync_InBlockedChat_CannotSendMessage()
    {
        var chat = new Chat { IsBlocked = true, User = new User { UserId = 1 } };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => chatService.SendMessageAsync(chatId: 1, content: "hello", senderId: 1, MessageType.Text));
    }

    [Fact]
    public async Task UnblockChatAsync_CallerDidNotBlockTheChat_CannotUnblock()
    {
        var chat = new Chat
        {
            User = new User { UserId = 1 },
            SecondUser = new User { UserId = 2 },
            IsBlocked = true,
            BlockedByUserId = 2,
        };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => chatService.UnblockChatAsync(chatId: 1, unblockerId: 1));
    }

    [Fact]
    public async Task UnblockChatAsync_CallerIsTheBlocker_UnblocksSuccessfully()
    {
        var chat = new Chat
        {
            User = new User { UserId = 1 },
            IsBlocked = true,
            BlockedByUserId = 1,
        };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        await chatService.UnblockChatAsync(chatId: 1, unblockerId: 1);

        Assert.False(chat.IsBlocked);
        Assert.Null(chat.BlockedByUserId);
    }


    [Fact]
    public async Task DeleteChatAsync_CallerIsFirstUser_OnlySetsDeletedAtByUser()
    {
        var chat = new Chat { User = new User { UserId = 1 }, SecondUser = new User { UserId = 2 } };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        await chatService.DeleteChatAsync(chatId: 1, callerId: 1);

        Assert.NotNull(chat.DeletedAtByUser);
        Assert.Null(chat.DeletedAtBySecondParty);
    }

    [Fact]
    public async Task DeleteChatAsync_CallerIsSecondUser_OnlySetsDeletedAtBySecondParty()
    {
        var chat = new Chat { User = new User { UserId = 1 }, SecondUser = new User { UserId = 2 } };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        await chatService.DeleteChatAsync(chatId: 1, callerId: 2);

        Assert.NotNull(chat.DeletedAtBySecondParty);
        Assert.Null(chat.DeletedAtByUser);
    }

        [Fact]
        public async Task GetMessagesAsync_NoDeletionTimestamp_AllMessagesAreVisible()
        {
            var callerId = 1;
            var chat = new Chat { User = new User { UserId = callerId }, DeletedAtByUser = null };
            var messages = new List<Message>
            {
                new() { Timestamp = DateTime.UtcNow.AddHours(-2), Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } },
                new() { Timestamp = DateTime.UtcNow.AddHours(-1), Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } },
            };

            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);
            messageRepository.GetForChatAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Message>>(messages));

            var result = await chatService.GetMessagesAsync(chatId: 1, callerId);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetMessagesAsync_MessageExactlyAtDeletionTimestamp_IsNotVisible()
        {
            var deletedAt = new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);
            var callerId = 1;
            var chat = new Chat { User = new User { UserId = callerId }, DeletedAtByUser = deletedAt };

            var beforeDeletion = new Message { Timestamp = deletedAt.AddSeconds(-1), Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } };
            var exactlyAtDeletion = new Message { Timestamp = deletedAt, Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } };
            var afterDeletion = new Message { Timestamp = deletedAt.AddSeconds(1), Sender = new MessageSender { SenderId = 2 }, Chat = new Chat { ChatId = 1 } };

            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);
            messageRepository.GetForChatAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Message>>(new List<Message> { beforeDeletion, exactlyAtDeletion, afterDeletion }));

            var result = await chatService.GetMessagesAsync(chatId: 1, callerId);

            Assert.Single(result);
            Assert.Equal(afterDeletion.Timestamp, result[0].Timestamp);
        }

        #endregion GetMessagesAsync

        #region SearchCompaniesAsync and SearchUsersAsync
        [Fact]
        public async Task SearchCompaniesAsync_EmptySearchTerm_ReturnsEmpty()
        {
            var result = await chatService.SearchCompaniesAsync("   ");

        await chatService.DeleteChatAsync(chatId: 1, callerId: 1);
        await chatService.DeleteChatAsync(chatId: 1, callerId: 2);

        Assert.NotNull(chat.DeletedAtByUser);
        Assert.NotNull(chat.DeletedAtBySecondParty);
    }

        [Fact]
        public async Task SearchCompaniesAsync_MatchingTerm_ReturnsFilteredCompanies()
        {
            var availableCompanies = new List<Company>
            {
                new() { Name = "Acme Corp" },
                new() { Name = "Other Company" }
            };

        var result = await chatService.GetChatsForUserAsync(userId);

            var result = await chatService.SearchCompaniesAsync("acme");

            Assert.Equal("Acme Corp", Assert.Single(result).Name);
        }

        [Fact]
        public async Task SearchCompaniesAsync_MoreThanMaxResults_ReturnsOnlyMaxResults()
        {
            var companies = Enumerable.Range(1, 15)
                .Select(companyNumber => new Company { Name = $"Company {companyNumber}" })
                .ToList();

        var result = await chatService.GetChatsForUserAsync(userId);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetChatsForUser_SoftDeletedChat_IsNotVisible()
    {
        var userId = 1;
        var chat = new Chat
        {
            User = new User { UserId = userId },
            DeletedAtByUser = DateTime.UtcNow,
        };
        chatRepository.GetForUserAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Chat>>(new List<Chat> { chat }));

        var result = await chatService.GetChatsForUserAsync(userId);

        Assert.Empty(result);
    }


    [Fact]
    public async Task SearchUsersAsync_RecruiterUsersAreExcludedFromResults()
    {
        var recruiterIds = new List<int> { 2 };
        var users = new List<User>
        {
            new() { UserId = 1, FirstName = "Alice", LastName = "Smith" },
            new() { UserId = 2, FirstName = "Alice", LastName = "Recruiter" },
        };

        recruiterRepository.GetAllRecruiterUserIdsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyCollection<int>>(recruiterIds));
        userService.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<User>>(users));

        var result = await chatService.SearchUsersAsync("Alice");

        Assert.Single(result);
        Assert.Equal(1, result[0].UserId);
    }

    [Fact]
    public async Task SearchUsersAsync_ResultsExceedCap_OnlyTenReturned()
    {
        recruiterRepository.GetAllRecruiterUserIdsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyCollection<int>>(new List<int>()));

        var users = Enumerable.Range(1, 20)
            .Select(i => new User { UserId = i, FirstName = "Bob", LastName = $"Smith {i}" })
            .ToList();
        userService.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<User>>(users));

        var result = await chatService.SearchUsersAsync("Bob");

            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task SearchUsersAsync_ExcludesRecruiters()
        {
            var users = new List<User>
            {
                new() { UserId = 1, FirstName = "Peter", LastName = "Pann" },
                new() { UserId = 2, FirstName = "Peter", LastName = "Recruiter" },
            };

            recruiterRepository.GetAllRecruiterUserIdsAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyCollection<int>>(new List<int> { 2 }));
            userService.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<User>>(users));

            var result = await chatService.SearchUsersAsync("peter");

            Assert.Equal(1, Assert.Single(result).UserId);
        }

        [Fact]
        public async Task SearchRecruitersByCompanyAsync_EmptySearchTerm_ReturnsEmpty()
        {
            var result = await chatService.SearchRecruitersByCompanyAsync(1, "   ");

            Assert.Empty(result);
            await userService.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task SearchRecruitersByCompanyAsync_MatchingTerm_ReturnsOnlyCompanyRecruitersMatchingName()
        {
            var users = new List<User>
            {
                new() { UserId = 1, FirstName = "Peter", LastName = "Pann" },     // company recruiter, name matches
                new() { UserId = 2, FirstName = "Tink", LastName = "Bell" },      // company recruiter, name does not match
                new() { UserId = 3, FirstName = "Peter", LastName = "Outsider" }, // name matches but not a company recruiter
            };

            recruiterRepository.GetUserIdsByCompanyAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<int>>(new List<int> { 1, 2 }));
            userService.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<User>>(users));

            var result = await chatService.SearchRecruitersByCompanyAsync(1, "peter");

            Assert.Equal(1, Assert.Single(result).UserId);
        }

        [Fact]
        public async Task SearchRecruitersByCompanyAsync_MoreThanMaxResults_ReturnsOnlyMaxResults()
        {
            var users = Enumerable.Range(1, 15)
                .Select(userNumber => new User { UserId = userNumber, FirstName = "Peter", LastName = $"Pann {userNumber}" })
                .ToList();

            recruiterRepository.GetUserIdsByCompanyAsync(1, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<int>>(Enumerable.Range(1, 15).ToList()));
            userService.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<User>>(users));

            var result = await chatService.SearchRecruitersByCompanyAsync(1, "peter");

            Assert.Equal(10, result.Count);
        }

        [Fact]
        public async Task SearchCompaniesAsync_SearchIsNotCaseSensitive()
        {
            var companies = new List<Company> { new() { Name = "Acme Corp" } };
            companyService.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Company>>(companies));

            var result = await chatService.SearchCompaniesAsync("ACME");

            Assert.Single(result);
        }
        #endregion


    [Fact]
    public async Task SearchCompaniesAsync_ResultsExceedCap_OnlyTenReturned()
    {
        var companies = Enumerable.Range(1, 20)
            .Select(i => new Company { Name = $"Acme {i}" })
            .ToList();
        companyService.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Company>>(companies));

        var result = await chatService.SearchCompaniesAsync("Acme");

        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task SearchCompaniesAsync_SearchIsNotCaseSensitive()
    {
        var companies = new List<Company> { new() { Name = "Acme Corp" } };
        companyService.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Company>>(companies));

        var result = await chatService.SearchCompaniesAsync("ACME");

        Assert.Single(result);
    }


    [Fact]
    public async Task SendMessageAsync_ImageWithDisallowedExtension_Throws()
    {
        var chat = new Chat { User = new User { UserId = 1 } };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        var tempFile = CreateTempFileWithExtension(".gif");
        try
        {
            await Assert.ThrowsAsync<NotSupportedException>(
                () => chatService.SendMessageAsync(chatId: 1, content: tempFile, senderId: 1, MessageType.Image));
        }
        finally { File.Delete(tempFile); }
    }

    [Fact]
    public async Task SendMessageAsync_FileWithDisallowedExtension_Throws()
    {
        var chat = new Chat { User = new User { UserId = 1 } };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        var tempFile = CreateTempFileWithExtension(".exe");
        try
        {
            await Assert.ThrowsAsync<NotSupportedException>(
                () => chatService.SendMessageAsync(chatId: 1, content: tempFile, senderId: 1, MessageType.File));
        }
        finally { File.Delete(tempFile); }
    }

    [Fact]
    public async Task SendMessageAsync_ImageExceedsSizeLimit_Throws()
    {
        var chat = new Chat { User = new User { UserId = 1 } };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        var tempFile = CreateTempFileWithExtension(".jpg", sizeBytes: 11 * 1024 * 1024); // 11 MB > 10 MB limit
        try
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => chatService.SendMessageAsync(chatId: 1, content: tempFile, senderId: 1, MessageType.Image));
        }
        finally { File.Delete(tempFile); }
    }

    [Fact]
    public async Task SendMessageAsync_TextExceedsMaxLength_Throws()
    {
        var chat = new Chat { User = new User { UserId = 1 } };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        var tooLong = new string('x', 2001);

        await Assert.ThrowsAsync<ArgumentException>(
            () => chatService.SendMessageAsync(chatId: 1, content: tooLong, senderId: 1, MessageType.Text));
    }

    [Fact]
    public async Task SendMessageAsync_TextAtExactMaxLength_DoesNotThrow()
    {
        var chat = new Chat { User = new User { UserId = 1 } };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        var exactLimit = new string('x', 2000);

        await chatService.SendMessageAsync(chatId: 1, content: exactLimit, senderId: 1, MessageType.Text);

        await messageRepository.Received(1).AddAsync(Arg.Any<Message>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendStoredAttachmentAsync_AddsMessage_WhenCallerIsParticipant()
    {
        var chat = new Chat { User = new User { UserId = 1 }, SecondUser = new User { UserId = 2 }, IsBlocked = false };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        var storedPath = "/stored/path/file.jpg";
        var originalFileName = "file.jpg";
        await chatService.SendStoredAttachmentAsync(chatId: 1, storedPath: storedPath, originalFileName: originalFileName, senderId: 1, MessageType.Image);

                await messageRepository.Received(1).AddAsync(
                    Arg.Is<Message>(message => message.Content == "stored/path.jpg" && message.OriginalFileName == Path.GetFileName(sentAttachmentPath)),
                    Arg.Any<CancellationToken>());
            }
            finally
            {
                File.Delete(sentAttachmentPath);
            }
        }

        [Fact]
        public async Task SendMessageAsync_AttachmentFileDoesNotExist_ThrowsFileNotFoundException()
        {
            var chat = new Chat { User = new User { UserId = 1 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await Assert.ThrowsAsync<FileNotFoundException>(
                () => chatService.SendMessageAsync(1, "Z:/no/such/file.png", 1, MessageType.Image));
        }

        [Fact]
        public async Task SendMessageAsync_AttachmentWrongExtensionForType_ThrowsNotSupportedException()
        {
            var chat = new Chat { User = new User { UserId = 1 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            var tempFile = Path.GetTempFileName();
            var textPath = Path.ChangeExtension(tempFile, ".txt");
            File.Move(tempFile, textPath);
            await File.WriteAllBytesAsync(textPath, new byte[10]);

            try
            {
                await Assert.ThrowsAsync<NotSupportedException>(
                    () => chatService.SendMessageAsync(1, textPath, 1, MessageType.Image));
            }
            finally
            {
                File.Delete(textPath);
            }
        }

        [Fact]
        public async Task SendMessageAsync_FileWithDisallowedExtension_ThrowsNotSupportedException()
        {
            var chat = new Chat { User = new User { UserId = 1 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            var tempFile = CreateTempFileWithExtension(".exe");
            try
            {
                await Assert.ThrowsAsync<NotSupportedException>(
                    () => chatService.SendMessageAsync(1, tempFile, 1, MessageType.File));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task SendMessageAsync_ImageExceedsSizeLimit_ThrowsInvalidOperationException()
        {
            var chat = new Chat { User = new User { UserId = 1 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            var tempFile = CreateTempFileWithExtension(".jpg", sizeBytes: 11 * 1024 * 1024); // 11 MB > 10 MB limit
            try
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => chatService.SendMessageAsync(1, tempFile, 1, MessageType.Image));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task SendMessageAsync_TextAtExactMaxLength_DoesNotThrow()
        {
            var chat = new Chat { User = new User { UserId = 1 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            var exactLimit = new string('x', 2000);

            await chatService.SendMessageAsync(1, exactLimit, 1, MessageType.Text);

            await messageRepository.Received(1).AddAsync(Arg.Any<Message>(), Arg.Any<CancellationToken>());
        }
        #endregion

        #region SendStoredAttachmentAsync
        [Fact]
        public async Task SendStoredAttachmentAsync_ChatNotFound_ThrowsKeyNotFoundException()
        {
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Chat?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => chatService.SendStoredAttachmentAsync(1, "stored/path.jpg", "photo.jpg", 1, MessageType.Image));
        }

        [Fact]
        public async Task SendStoredAttachmentAsync_CallerIsNotParticipant_ThrowsUnauthorizedAccessException()
        {
            var chat = new Chat { User = new User { UserId = 2 }, SecondUser = new User { UserId = 3 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => chatService.SendStoredAttachmentAsync(1, "stored/path.jpg", "photo.jpg", 99, MessageType.Image));
        }

        [Fact]
        public async Task SendStoredAttachmentAsync_BlockedChat_ThrowsInvalidOperationException()
        {
            var chat = new Chat { IsBlocked = true, User = new User { UserId = 1 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => chatService.SendStoredAttachmentAsync(1, "stored/path.jpg", "photo.jpg", 1, MessageType.Image));
        }

        [Fact]
        public async Task SendStoredAttachmentAsync_ValidRequest_AddsMessageWithStoredPathAndFileName()
        {
            var chat = new Chat { User = new User { UserId = 1 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await chatService.SendStoredAttachmentAsync(1, "stored/path.jpg", "photo.jpg", 1, MessageType.Image);

            await messageRepository.Received(1).AddAsync(
                Arg.Is<Message>(message => message.Content == "stored/path.jpg" && message.OriginalFileName == "photo.jpg"),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task SendStoredAttachmentAsync_CompanyRepresentative_AddsMessage()
        {
            var chat = new Chat { User = new User { UserId = 1 }, Company = new Company { CompanyId = 100 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await chatService.SendStoredAttachmentAsync(1, "stored/doc.pdf", "doc.pdf", 999, MessageType.File, companyId: 100);

            await messageRepository.Received(1).AddAsync(
                Arg.Is<Message>(message => message.Content == "stored/doc.pdf"
                    && message.OriginalFileName == "doc.pdf"
                    && message.Type == MessageType.File),
                Arg.Any<CancellationToken>());
        }
        #endregion

        [Fact]
        public async Task OpenMessageAttachmentAsync_EmptyPath_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => chatService.OpenMessageAttachmentAsync("   "));
        }

    [Fact]
    public async Task SendStoredAttachmentAsync_AllowsCompanyRepresentative()
    {
        var chat = new Chat { User = new User { UserId = 1 }, Company = new Company { CompanyId = 100 }, IsBlocked = false };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        var storedPath = "/stored/path/doc.pdf";
        var originalFileName = "doc.pdf";
        await chatService.SendStoredAttachmentAsync(chatId: 1, storedPath: storedPath, originalFileName: originalFileName, senderId: 999, MessageType.File, companyId: 100);

        await messageRepository.Received(1).AddAsync(
            Arg.Is<Message>(m =>
                m.Chat.ChatId == 1 &&
                m.Sender.SenderId == 999 &&
                m.Content == storedPath &&
                m.OriginalFileName == originalFileName &&
                m.Type == MessageType.File),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendStoredAttachmentAsync_ChatNotFound_ThrowsKeyNotFoundException()
    {
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Chat?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            chatService.SendStoredAttachmentAsync(chatId: 1, storedPath: "p", originalFileName: "o", senderId: 1, MessageType.File));
    }

    [Fact]
    public async Task SendStoredAttachmentAsync_InBlockedChat_ThrowsInvalidOperationException()
    {
        var chat = new Chat { User = new User { UserId = 1 }, IsBlocked = true };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            chatService.SendStoredAttachmentAsync(chatId: 1, storedPath: "p", originalFileName: "o", senderId: 1, MessageType.File));
    }

    [Fact]
    public async Task SendStoredAttachmentAsync_CallerNotParticipant_ThrowsUnauthorizedAccessException()
    {
        var chat = new Chat { User = new User { UserId = 1 }, SecondUser = new User { UserId = 2 }, IsBlocked = false };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await chatService.BlockChatAsync(1, blockerId: 1);

            Assert.True(chat.IsBlocked);
            Assert.Equal(1,chat.BlockedByUserId);
            Assert.Equal(1, chat.BlockedByUser!.UserId);
            await chatRepository.Received(1).UpdateAsync(chat, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task BlockChatAsync_SecondUserBlocks_SetsBlockedByUserToSecondUser()
        {
            var secondUser = new User { UserId = 2 };
            var chat = new Chat { User = new User { UserId = 1 }, SecondUser = secondUser };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await chatService.BlockChatAsync(1, blockerId: 2);

            Assert.True(chat.IsBlocked);
            Assert.Equal(2, chat.BlockedByUserId);
            Assert.Same(secondUser, chat.BlockedByUser);
            await chatRepository.Received(1).UpdateAsync(chat, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task BlockChatAsync_CallerIsCompanyRepresentative_BlocksChat()
        {
            var chat = new Chat { User = new User { UserId = 1 }, Company = new Company { CompanyId = 100 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            // The company rep is not a loaded chat participant, so BlockChatAsync resolves
            // them via GetUserAsync — userService must contain that user.
            userService.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<User>>(new List<User> { new() { UserId = 999 } }));

            await chatService.BlockChatAsync(1, blockerId: 999, companyId: 100);

            Assert.True(chat.IsBlocked);
            Assert.Equal(999, chat.BlockedByUserId);
            await chatRepository.Received(1).UpdateAsync(chat, Arg.Any<CancellationToken>());
        }

    [Fact]
    public async Task BlockChatAsync_CallerIsParticipant_BlocksChat()
    {
        var chat = new Chat
        {
            User = new User { UserId = 1 },
            SecondUser = new User { UserId = 2 },
            IsBlocked = false
        };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        await chatService.BlockChatAsync(chatId: 1, blockerId: 1);

        Assert.True(chat.IsBlocked);
        Assert.Equal(1, chat.BlockedByUserId);
        await chatRepository.Received(1).UpdateAsync(chat, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BlockChatAsync_CallerIsCompanyRepresentative_BlocksChat()
    {
        var chat = new Chat
        {
            var chat = new Chat { User = new User { UserId = 1 }, BlockedByUser = new User { UserId = 1 }, IsBlocked = true, BlockedByUserId = 1};
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        // Ensure the user service contains the company representative so GetUserAsync won't throw.
        userService.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<User>>(new List<User> { new User { UserId = 999 } }));

        await chatService.BlockChatAsync(chatId: 1, blockerId: 999, companyId: 100);

            Assert.False(chat.IsBlocked);
            Assert.Null(chat.BlockedByUserId);
            Assert.Null(chat.BlockedByUser);
            await chatRepository.Received(1).UpdateAsync(chat, Arg.Any<CancellationToken>());
        }

    [Fact]
    public async Task BlockChatAsync_ChatNotFound_ThrowsKeyNotFoundException()
    {
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Chat?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            chatService.BlockChatAsync(chatId: 1, blockerId: 1));
    }

    [Fact]
    public async Task BlockChatAsync_CallerNotParticipant_ThrowsUnauthorizedAccessException()
    {
        var chat = new Chat
        {
            User = new User { UserId = 1 },
            SecondUser = new User { UserId = 2 },
            IsBlocked = false
        };
        chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            chatService.BlockChatAsync(chatId: 1, blockerId: 3));
    }

        [Fact]
        public async Task DeleteChatAsync_CallerIsUser_SetsDeletedAtByUser()
        {
            var chat = new Chat { User = new User { UserId = 1 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await chatService.DeleteChatAsync(1, callerId: 1);

            Assert.NotNull(chat.DeletedAtByUser);
            Assert.Null(chat.DeletedAtBySecondParty);
            await chatRepository.Received(1).UpdateAsync(chat, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteChatAsync_CallerIsSecondParty_SetsDeletedAtBySecondParty()
        {
            var chat = new Chat { User = new User { UserId = 1 }, SecondUser = new User { UserId = 2 } };
            chatRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(chat);

            await chatService.DeleteChatAsync(1, callerId: 2);

            Assert.NotNull(chat.DeletedAtBySecondParty);
            Assert.Null(chat.DeletedAtByUser);
            await chatRepository.Received(1).UpdateAsync(chat, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteChatAsync_BothPartiesDelete_BothFlagsAreSet()
        {
            var chat = new Chat { User = new User { UserId = 1 }, SecondUser = new User { UserId = 2 } };
            chatRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(chat);

            await chatService.DeleteChatAsync(1, callerId: 1);
            await chatService.DeleteChatAsync(1, callerId: 2);

            Assert.NotNull(chat.DeletedAtByUser);
            Assert.NotNull(chat.DeletedAtBySecondParty);
        }

        private static string CreateTempFileWithExtension(string extension, int sizeBytes = 100)
        {
            var path = Path.ChangeExtension(Path.GetTempFileName(), extension);
            File.WriteAllBytes(path, new byte[sizeBytes]);
            return path;
        }
    }
}