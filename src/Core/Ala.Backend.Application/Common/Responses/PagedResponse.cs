namespace Ala.Backend.Application.Common.Responses
{
    public class PagedResponse<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => TotalCount == 0
            ? 0
            : (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}