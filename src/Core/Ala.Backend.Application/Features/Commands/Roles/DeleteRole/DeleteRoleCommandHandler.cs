using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Roles.DeleteRole
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommandRequest, SuccessDetails>
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService; 

        public DeleteRoleCommandHandler(IRoleService roleService, IUserService userService)
        {
            _roleService = roleService;
            _userService = userService; 
        }

        public async Task<SuccessDetails> Handle(DeleteRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleService.FindByIdAsync(request.Id.ToString());
            if (role is null)
                throw new NotFoundException("Silinmek istenen rol sistemde bulunamadı.");

            var usersInRole = await _userService.GetUsersInRoleAsync(role.Name!);

            if (usersInRole.Any()) 
            {
                throw new ConflictException("Bu role atanmış mevcut kullanıcılar var. Rolü silebilmek için önce o kullanıcıların rollerini değiştirin veya sistemden kaldırın.");
            }

            var result = await _roleService.DeleteAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Rol silinirken hata meydana geldi: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}