using FluentValidation;
using MusicServer.Core.Const;
using MusicServer.Entities.Requests.Song;

namespace MusicServer.Validation
{
    public class SearchValidator : AbstractValidator<Search>
    {
        public SearchValidator()
        {
            RuleFor(x => x.Filter).NotEmpty().Must(x => SearchFilter.SearchFilters.Contains(x));
            RuleFor(x => x.SearchTerm).MaximumLength(1024);
        }
    }
}
