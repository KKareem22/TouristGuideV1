namespace TouristGuide.Application.Common
{
    public class PlaceSpecParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageIndex { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
        }

        public string? Category { get; set; }
        public bool? Featured { get; set; }
    }
}
