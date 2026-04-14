namespace Ala.Backend.Application.DTOs.Users
{
    public class UserListFilter
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public string? Search { get; init; }
        public string? SortBy { get; init; }
        public string? SortDirection { get; init; }
        public bool? IsActive { get; init; }
        public string? Role { get; init; }
    }
}
