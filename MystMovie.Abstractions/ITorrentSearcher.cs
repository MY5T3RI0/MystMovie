using MystMovie.DB;

namespace MystMovie.Services
{
	public interface ITorrentSearcher
	{
		public string MainUrl { get; set; }

		public Task<List<Movie>> LoadLinksByGenre(Genre genre);
		public Task<List<Genre>> GetGenres();
	}
}