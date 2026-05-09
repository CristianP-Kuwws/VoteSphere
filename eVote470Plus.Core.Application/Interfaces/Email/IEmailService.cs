using eVote470Plus.Core.Application.Dtos.Email;

namespace eVote470Plus.Core.Application.Interfaces.Email
{
    public interface IEmailService
    {
        public Task SendAsync(EmailRequestDto emailRequest);
    }
}
