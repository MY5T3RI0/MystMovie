using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MystMovie.DB;
using MystMovie.Services;

namespace MystMovie.TorrentService.Controllers
{
	[Route("api/[controller]")]
	public class MovieController : Controller
	{
		private readonly ILogger<MovieController> _logger;
		private readonly ITorrentSearcher _searcher;
		private readonly MovieContext _movieContext;

		public MovieController(
			ILogger<MovieController> logger,
			ITorrentSearcher searcher,
			MovieContext movieContext)
		{
			_logger = logger;
			_searcher = searcher;
			_movieContext = movieContext;
		}

		[HttpGet("id")]
		public Movie? Get(Guid id) 
			=> _movieContext.Movies.SingleOrDefault(movie => movie.ID == id);

		[HttpGet]
		public async Task<List<Movie>> GetAllAsync(int from = 0, int to = 10)
			=> await _movieContext.Movies.Skip(from).Take(to).ToListAsync();

		[HttpGet("genreId")]
		public async Task<List<Movie>> GetByGenreAsync(Guid genreId, int from = 0, int to = 10) 
			=> await _movieContext.Movies
				.Where(movie => movie.GenreId == genreId)
				.Skip(from)
				.Take(to)
				.ToListAsync();

		[HttpPost]
		public async Task<ActionResult> UpdateMoviesAll([FromBody] List<Genre> genres)
		{
			try
			{
				foreach (var genre in genres)
					await UpdateMoviesByGenre(genre);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				return BadRequest(ex.Message);
			}
			
			return Ok();
		}

		private async Task UpdateMoviesByGenre(Genre genre)
		{
			var movies = await _searcher.LoadLinksByGenre(genre);

			foreach (var movie in movies)
			{
				if (!_movieContext.Movies.Contains(movie))
					_movieContext.Movies.Add(movie);
			}

			await _movieContext.SaveChangesAsync();
		}
	}
}
