using System.ComponentModel.DataAnnotations.Schema;

namespace MystMovie.DB
{
	[Table("Movie")]
	public class Movie : EntityBase
	{
        public string Name { get; set; }
		public string? Quality { get; set; }
        public Guid GenreId { get; set; }
        public string Link { get; set; }
		public int Year { get; set; } = 0;
		public string? Description { get; set; }
		public int MovieLength { get; set; } = 0;
		public string? Rating { get; set; }
		public int AgeRating { get; set; } = 0;
        public string? ImageUrl { get; set; }
        public string? Country { get; set; }

        [ForeignKey("GenreId")]
		public Genre Genre { get; set; }
	}
}
