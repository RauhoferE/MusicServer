using FluentValidation;
using MusicServer.Entities.Requests.Song;

namespace MusicServer.Validation
{

    public class UpdatePlaylistValidator : AbstractValidator<UpdatePlaylist>
    {
        public UpdatePlaylistValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(1024);
        }
    }
}
