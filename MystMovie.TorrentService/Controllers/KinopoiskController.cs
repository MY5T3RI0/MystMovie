
using Microsoft.AspNetCore.Mvc;
using MystMovie.DB;
using Newtonsoft.Json;

namespace MystMovie.TorrentService.Controllers
{
	[Route("api/[controller]")]
	public class KinopoiskController : Controller
	{
		private MovieContext _movieContext;

        public KinopoiskController(MovieContext movieContext)
        {
            _movieContext = movieContext;
        }

		[HttpPost]
		public async Task<IActionResult> UpdateMoviesAllAsync(int from = 1, int to = 200)
		{
			var totalChanged = 0;

			for (int i = from; i < to; i++)
			{
				var result =  await UpdateMoviesAsync(i);

				if (result is ObjectResult objectResult)
					if (objectResult.Value is int count)
						totalChanged += count;
			}

			return Ok(totalChanged);
		}

        [HttpPost("pageNumber")]
		public async Task<IActionResult> UpdateMoviesAsync(int pageNumber)
		{
			var url = $"https://api.kinopoisk.dev/v1.4/movie/search?page={pageNumber}&limit=250";
			var moviesChanged = 0;

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Add("X-API-KEY", "0K4QC8G-BS8M0NR-JAF87SB-6H5VM55");

				var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
				var json = await response.Content.ReadAsStringAsync();
				var moviesInfo = JsonConvert.DeserializeObject<MoviesInfo> (json)?.docs;

				if(moviesInfo != null) 
					foreach (var info in moviesInfo)
					{
						var movie = _movieContext.Movies.FirstOrDefault(movie => movie.Name == info.name);

						if (movie != null)
						{
							movie.Year = info.year;
							movie.Description = info.description;
							movie.MovieLength = info.movieLength;
							movie.Rating = info.rating.kp.ToString();
							movie.AgeRating = info.ageRating ?? 0;
							movie.ImageUrl = info.poster.url;
							movie.Country = info.countries?.FirstOrDefault()?.name ?? "Undefined";

							_movieContext.Movies.Update(movie);
							await _movieContext.SaveChangesAsync();
							moviesChanged++;
						}
					}
			}

			return Ok(moviesChanged);
		}
	}
}