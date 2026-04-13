using Ala.Backend.Domain.Common;

namespace Ala.Backend.Domain.Identity
{
    public class Permission : TrackableEntity<int>
    {
        public string Code { get; set; } =null!;
        public string? Description { get; set; }
    }
}
