using System.ComponentModel.DataAnnotations.Schema;

namespace MystMovie.DB
{
	[Table("Genre")]
	public class Genre : EntityBase
	{
        public string Name { get; set; }
        public string? Link { get; set; }
    }
}