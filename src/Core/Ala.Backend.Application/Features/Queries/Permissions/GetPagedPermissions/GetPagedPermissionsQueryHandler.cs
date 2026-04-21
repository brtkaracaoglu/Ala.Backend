using Ala.Backend.Application.Abstractions.Persistence.Repositories.Enitties;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Permissions;
using Ala.Backend.Application.SystemMessages;
using Ala.Backend.Domain.Identity;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Permissions.GetPagedPermissions
{
    public class GetPagedPermissionsQueryHandler : IRequestHandler<GetPagedPermissionsQueryRequest, SuccessDetails<PagedResponse<PermissionDto>>>
    {
        private readonly IReadRepository<Permission, int> _readRepository;

        public GetPagedPermissionsQueryHandler(IReadRepository<Permission, int> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<SuccessDetails<PagedResponse<PermissionDto>>> Handle(GetPagedPermissionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _readRepository.GetPagedAsync(
                request,
                query =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Search))
                    {
                        var search = request.Search.Trim();

                        query = query.Where(x =>
                            (x.Code != null && x.Code.Contains(search)) ||
                            (x.Description != null && x.Description.Contains(search)));
                    }

                    query = request.SortBy?.ToLower() switch
                    {
                        "code" => request.Desc
                            ? query.OrderByDescending(x => x.Code)
                            : query.OrderBy(x => x.Code),

                        "description" => request.Desc
                            ? query.OrderByDescending(x => x.Description)
                            : query.OrderBy(x => x.Description),

                        _ => query.OrderBy(x => x.Id)
                    };

                    return query;
                },
                x => new PermissionDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                },
                cancellationToken: cancellationToken);

            return ResultResponse.Success(result, Response.Common.OperationSuccess);
        }
    }
}