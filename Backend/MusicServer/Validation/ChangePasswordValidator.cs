using FluentValidation;
using MusicServer.Entities.Requests.User;

namespace MusicServer.Validation
{
    public class ChangePasswordValidator : AbstractValidator<ChangePassword>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty().Matches("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$");
        }
    }
}
