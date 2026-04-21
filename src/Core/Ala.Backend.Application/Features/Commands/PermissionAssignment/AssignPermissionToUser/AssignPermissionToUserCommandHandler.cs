using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.UnitOfWork;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using Ala.Backend.Domain.Identity;
using MediatR;
using System.Security.Claims;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.AssignPermissionToUser
{
    public class AssignPermissionToUserCommandHandler : IRequestHandler<AssignPermissionToUserCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public AssignPermissionToUserCommandHandler(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<SuccessDetails> Handle(AssignPermissionToUserCommandRequest request, CancellationToken cancellationToken)
        {
            var permissionRepo = _unitOfWork.ReadRepository<Permission, int>();

            var permissionExists = await permissionRepo.AnyAsync(x => x.Code == request.PermissionCode, cancellationToken);
            if (!permissionExists)
                throw new NotFoundException("Belirtilen yetki kodu sistemde bulunamadı.");

            var user = await _userService.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            var result = await _userService.AddClaimAsync(user, new Claim("permission", request.PermissionCode));

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Yetki kullanıcıya atanırken bir hata oluştu: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}
