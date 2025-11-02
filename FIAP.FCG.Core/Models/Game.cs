
namespace FIAP.FCG.Core.Models
{
	public class Game : EntityBase
	{
		public required string Name { get; set; }
		public required string Platform { get; set; }
        public required string PublisherName { get; set; }
		public required string Description { get; set; }
        public required double Price { get; set; }
	}
}
