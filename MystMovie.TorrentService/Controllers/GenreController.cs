using Microsoft.AspNetCore.Mvc;
using MystMovie.DB;
using MystMovie.Services;

namespace MystMovie.TorrentService.Controllers
{
	[Route("api/[controller]")]
	public class GenreController : ControllerBase
	{
		private readonly ILogger<GenreController> _logger;
		private readonly ITorrentSearcher _searcher;
		private readonly MovieContext _movieContext;

		public GenreController(
			ILogger<GenreController> logger,
			ITorrentSearcher searcher,
			MovieContext movieContext)
		{
			_logger = logger;
			_searcher = searcher;
			_movieContext = movieContext;
		}

		[HttpGet("id")]
		public Genre Get(Guid genreId)
			=> _movieContext.Genres.SingleOrDefault(genre => genre.ID == genreId) ?? new Genre();

		[HttpGet]
		public async Task<ActionResult<List<Genre>>> GetAllActual(bool update = false)
		{
			if (update)
			{
				try
				{
					await UpdateGenres();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex.Message);

					return BadRequest(ex.Message);
				}
			}

			return _movieContext.Genres.ToList();
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAll()
		{
			try
			{
				await UpdateGenres();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				return BadRequest(ex.Message);
			}

			return Ok();
		}

		private async Task UpdateGenres()
		{
			foreach (var genre in await _searcher.GetGenres())
			{
				if (_movieContext.Genres.Contains(genre))
					_movieContext.Genres.Update(genre);
				else
					_movieContext.Genres.Add(genre);
			}

			await _movieContext.SaveChangesAsync();
		}
	}
}
