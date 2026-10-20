using Specialist.Assessment.Models;
using Specialist.Assessment.Services;

namespace Specialist.Assessment.Tests;

public class LeadServiceTests
{
    [Fact]
    public async Task SubmitLead_PrimaryServiceFails_LegalClient_UsesUflFallback()
    {
        // Arrange
        var mockEmailService = new FailingEmailService();
        var service = new LeadService(mockEmailService);
        var dto = new CreateLeadDto
        {
            Name = "Corp Client",
            Phone = "+79001234567",
            Email = "client@corp.com",
            ClientType = ClientType.Legal
        };

        // Act
        var result = await service.SubmitLeadAsync(dto);

        // Assert
        Assert.True(result.Success);
        // При Legal клиенте в catch: dto.ClientType != ClientType.Legal = false → IndividualFallbackEmail
        Assert.Equal("specialist.ufl@yandex.ru", result.FallbackEmail);
    }

    [Fact]
    public async Task SubmitLead_PrimaryServiceFails_IndividualClient_UsesCorpFallback()
    {
        // Arrange
        var mockEmailService = new FailingEmailService();
        var service = new LeadService(mockEmailService);
        var dto = new CreateLeadDto
        {
            Name = "John Doe",
            Phone = "+79007654321",
            Email = "john@example.com",
            ClientType = ClientType.Individual
        };

        // Act
        var result = await service.SubmitLeadAsync(dto);

        // Assert
        Assert.True(result.Success);
        // При Individual клиенте в catch: dto.ClientType != ClientType.Legal = true → LegalFallbackEmail
        Assert.Equal("specialist.corp@yandex.ru", result.FallbackEmail);
    }

    /// <summary>
    /// Мокающий сервис, который всегда выбрасывает CustomStrangeException (имитация сбоя).
    /// </summary>
    private class FailingEmailService : IEmailService
    {
        public Task SendAsync(string fromEmail, string toEmail, string subject, string body)
        {
            throw new CustomStrangeException("Почтовый сервис недоступен");
        }
    }
}
