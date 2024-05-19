using System.ComponentModel.DataAnnotations;

namespace MystMovie.DB
{
	public abstract class EntityBase
	{
		[Key]
		public Guid ID { get; set; }
	}
}
