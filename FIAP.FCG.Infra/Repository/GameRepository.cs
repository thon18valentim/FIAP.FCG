using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using FIAP.FCG.Infra.Context;

namespace FIAP.FCG.Infra.Repository
{
	public class GameRepository(ApplicationDbContext context, IMapper mapper) : EFRepository<Game>(context), IGameRepository
	{
		private readonly IMapper _mapper = mapper;

        public async Task<GameRegisterDto> Create(GameRegisterDto gameRegister)
        {
            var game = await Create(gameRegister);
            return _mapper.Map<GameRegisterDto>(game);
        }

        public async Task<IEnumerable<GameResponseDto>> GetAll()
		{
			var games =  await Get();
            return [.. games.Select(g => _mapper.Map<GameResponseDto>(games))];
        }

        public async Task<GameResponseDto?> GetById(int id)
        {
            var game = await Get(id);
            return game is null ? null : _mapper.Map<GameResponseDto>(game);
        }

        public async Task<bool> Update(int id, GameUpdateDto gameUpdateDto)
        {
            var game = await Get(id) ?? throw new ArgumentNullException(nameof(id), $"Erro ao atualizar: Jogo inexistente!");
            _mapper.Map(gameUpdateDto, game);
            return await Edit(game);
        }

        public async Task<bool> Remove(int id)
        {
            return await Delete(id);
        }
    }
}
