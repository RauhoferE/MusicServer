using FluentValidation;
using MusicServer.Entities.Requests.User;

namespace MusicServer.Validation
{
    public class DeleteAccountValidator : AbstractValidator<DeleteAccount>
    {
        public DeleteAccountValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
