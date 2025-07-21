using AutoMapper;
using BluesenseChat.Application.Common;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Application.Interfaces.RepositoryInterfaces;
using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using BluesenseChat.Application.Services;
using BluesenseChat.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class MessageServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<MessageService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMongoMessageRepository> _mongoMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();

    private readonly MessageService _service;

    public MessageServiceTests()
    {
        _service = new MessageService(
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _mongoMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task SendMessageAsync_Should_Fail_If_Content_And_Attachments_Empty()
    {
        // Arrange
        var dto = new SendMessageDto
        {
            Content = "",
            Attachments = null
        };

        // Act
        var result = await _service.SendMessageAsync(dto, Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Mesaj içeriği veya dosya eklenmelidir");
    }

    [Fact]
    public async Task SendMessageAsync_Should_Fail_If_Sender_Not_Found()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User)null);

        var dto = new SendMessageDto
        {
            Content = "Merhaba"
        };

        // Act
        var result = await _service.SendMessageAsync(dto, Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Gönderen kullanıcı bulunamadı");
    }

    [Fact]
    public async Task SendMessageAsync_Should_Succeed_If_Valid_Message()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId });

        _mongoMock.Setup(m => m.AddAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask);

        _cacheMock.Setup(c => c.GetAsync<List<Message>>(It.IsAny<string>()))
            .ReturnsAsync(new List<Message>());

        var dto = new SendMessageDto
        {
            Content = "Merhaba"
        };

        // Act
        var result = await _service.SendMessageAsync(dto, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Content.Should().Be("Merhaba");
    }

    [Fact]
    public async Task SendMessageAsync_Should_Add_Attachment_If_File_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId });

        _unitOfWorkMock.Setup(u => u.Attachments.AddAsync(It.IsAny<Attachment>()))
            .Returns(Task.CompletedTask);

        _mongoMock.Setup(m => m.AddAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask);

        _cacheMock.Setup(c => c.GetAsync<List<Message>>(It.IsAny<string>()))
            .ReturnsAsync(new List<Message>());

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("test.pdf");
        fileMock.Setup(f => f.Length).Returns(1000);
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

        var dto = new SendMessageDto
        {
            Content = "Mesaj + dosya",
            Attachments = new List<IFormFile> { fileMock.Object }
        };

        // Act
        var result = await _service.SendMessageAsync(dto, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Attachments.Should().NotBeEmpty();
    }
    [Fact]
    public async Task SendMessageAsync_Should_Add_Message_To_Redis_Cache()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId });

        _mongoMock.Setup(m => m.AddAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask);

        _cacheMock.Setup(c => c.GetAsync<List<Message>>(It.IsAny<string>()))
            .ReturnsAsync(new List<Message>());

        var dto = new SendMessageDto
        {
            Content = "Redis testi"
        };

        // Act
        var result = await _service.SendMessageAsync(dto, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _cacheMock.Verify(c => c.SetAsync(
            It.Is<string>(key => key.StartsWith("private:") || key.StartsWith("group:")),
            It.IsAny<List<Message>>(),
            It.IsAny<TimeSpan>()),
            Times.Once);
    }

}
