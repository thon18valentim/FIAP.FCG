using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;
using FIAP.FCG.Infra.Repository;

namespace FIAP.FCG.Application.Services
{
    public class UserService(IUserRepository repository) : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository = repository;
        public async Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll()
        {
            var list = await _userRepository.GetAll();
            return Ok(list);
        }

        public async Task<IApiResponse<UserResponseDto?>> GetById(int id)
        {
            var dto = await _userRepository.GetById(id);
            return dto is null
                ? NotFound<UserResponseDto?>("Usuário não encontrado.")
                : Ok<UserResponseDto?>(dto);
        }

        public async Task<IApiResponse<bool>> Update(int id, UserUpdateDto dto)
        {
            var ok = await _userRepository.Update(id, dto);
            return ok
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para atualização.");
        }

        public async Task<IApiResponse<bool>> Remove(int id)
        {
            var ok = await _userRepository.Remove(id);
            return ok
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para remoção.");
        }
    }
}
