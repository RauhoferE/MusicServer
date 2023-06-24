using FluentValidation;
using MusicServer.Entities.Requests.Multi;

namespace MusicServer.Validation
{
    public class QueryPaginationSearchRequestValidator : AbstractValidator<QueryPaginationSearchRequest>
    {
        public QueryPaginationSearchRequestValidator()
        {
             RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1");

            RuleFor(x => x.Take)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Take must be greater than or equal to 1");
        }
    }
}
