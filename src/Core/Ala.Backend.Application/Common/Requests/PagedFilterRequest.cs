namespace Ala.Backend.Application.Common.Requests
{
    public abstract class PagedFilterRequest : PagedRequest
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool Desc { get; set; }
    }
}
