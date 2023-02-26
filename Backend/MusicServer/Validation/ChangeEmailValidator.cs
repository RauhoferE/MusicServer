using FluentValidation;
using MusicServer.Entities.Requests.User;

namespace MusicServer.Validation
{
    public class ChangeEmailValidator : AbstractValidator<ChangeEmail>
    {
        public ChangeEmailValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
