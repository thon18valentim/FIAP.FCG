using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;
using FIAP.FCG.Infra.Repository;

namespace FIAP.FCG.Application.Services
{
    public class UserService(IUserRepository repository) : BaseService, IUserService
    {
        public async Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll()
        {
            var list = await repository.GetAll();
            return Ok(list);
        }

        public async Task<IApiResponse<UserResponseDto?>> GetById(int id)
        {
            var dto = await repository.GetById(id);
            return dto is null
                ? NotFound<UserResponseDto?>("Usuário não encontrado.")
                : Ok<UserResponseDto?>(dto);
        }

        public async Task<IApiResponse<bool>> Update(int id, UserUpdateDto dto)
        {
            var ok = await repository.Update(id, dto);
            return ok
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para atualização.");
        }

        public async Task<IApiResponse<bool>> Remove(int id)
        {
            var ok = await repository.Remove(id);
            return ok
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para remoção.");
        }
    }
}
