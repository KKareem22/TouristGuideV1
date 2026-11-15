using TouristGuide.Application.Common;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Application.Specification
{
    public class TouristPlaceSpecification : BaseSpecifications<TouristPlace, int>
    {
        public TouristPlaceSpecification(PlaceSpecParams specParams, bool applyPagination = true)
        {
            PlaceCategory? parsedCategory = null;
            if (!string.IsNullOrWhiteSpace(specParams.Category) &&
                Enum.TryParse<PlaceCategory>(specParams.Category, true, out var cat))
            {
                parsedCategory = cat;
            }

            Criteria = p =>
                !p.IsDeleted &&
                (!parsedCategory.HasValue || p.Category == parsedCategory.Value) &&
                (!specParams.Featured.HasValue || p.IsFeatured == specParams.Featured.Value);

            OrderBy = p => p.Name;

            if (applyPagination)
            {
                ApplyPagination(
                    skip: (specParams.PageIndex - 1) * specParams.PageSize,
                    take: specParams.PageSize
                );
            }
        }
    }
}
