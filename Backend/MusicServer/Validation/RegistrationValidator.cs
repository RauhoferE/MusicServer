using FluentValidation;
using MusicServer.Entities.Requests.User;

namespace MusicServer.Validation
{
    public class RegistrationValidator : AbstractValidator<Register>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(255);
            RuleFor(x => x.Birth).Must(x => x <= DateTime.Now);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$");
            RuleFor(x => x.RegistrationCode).NotEmpty();
        }
    }
}
