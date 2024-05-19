using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using MystMovie.DB;
using MystMovie.Services;
using System.Security;
using System.Text.RegularExpressions;

namespace MystMovie.Picker
{
	public class TorrentSearcher : ITorrentSearcher
	{
		public string MainUrl { get; set; }
		private readonly MovieContext _movieContext;

		public TorrentSearcher(IConfiguration
			configuration, MovieContext context)
		{
			_movieContext = context;
			MainUrl = configuration.GetSection("TorrentMainPage").Value ?? "";
		}

		public async Task<List<Movie>> LoadLinksByGenre(Genre genre)
		{
			var movies = new List<Movie>();

			if (string.IsNullOrEmpty(genre.Link)) return movies;

			var genrePage = new HtmlDocument();
			var pageNumber = 1;

			var maxPage = 0;

			do
			{
				await GetPage(genrePage, MainUrl + genre.Link + $"/page/{pageNumber}/");

				if (genrePage == null) break;

				var moviesList = genrePage.DocumentNode.QuerySelector("#dle-content");

				if (moviesList == null)
					throw new Exception("Incorrect genre page structure");

				foreach (var movie in moviesList.QuerySelectorAll(".post"))
					await LoadMoviesByPage(movies, movie, genre);

				pageNumber++;

				if (maxPage == 0)
				{
					maxPage = Convert.ToInt32(genrePage.DocumentNode.QuerySelector(".navigation")
						?.QuerySelectorAll("a")?.Reverse()?.Skip(1)
						?.FirstOrDefault()?.InnerText);

					maxPage = maxPage == 0 ? 100 : maxPage;
				}

			} while (pageNumber < maxPage);

			return movies;
		}
		public async Task<List<Genre>> GetGenres()
		{
			var genres = new List<Genre>();

			var mainPage = new HtmlDocument();

			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, MainUrl));

				var html = await response.Content.ReadAsStringAsync();
				mainPage.LoadHtml(html);
			}

			var menu = mainPage.DocumentNode.QuerySelector(".menu");
			var links = menu?.QuerySelectorAll("a");

			if (links == null)
				throw new Exception("Incorrect main page structure");

			foreach (HtmlNode link in links)
				genres.Add(new Genre
				{
					Name = link.InnerText,
					Link = link.GetAttributeValue("href", null)
				});

			return genres;
		}

		private async Task LoadMoviesByPage(List<Movie> movies, HtmlNode movie, Genre genre)
		{
			var movieLink = movie?.QuerySelector("a")?.GetAttributeValue("href", null);

			if (movieLink == null) return;

			var moviePage = new HtmlDocument();

			await GetPage(moviePage, movieLink);

			var movieName = moviePage.DocumentNode.QuerySelector(".post-title").InnerText;

			if (movieName.Contains(" (сериал)"))
				movieName = movieName.Substring(0, movieName.IndexOf(" (сериал)"));

			if (movieName == "Внимание, обнаружена ошибка") return;

			var linksTable = moviePage.DocumentNode.QuerySelector(".res85gtj");
			var linksTableRows = linksTable?.QuerySelectorAll("tr");

			if (linksTableRows == null)
				throw new Exception($"Incorrect movie page structure");

			foreach (var row in linksTableRows)
			{
				var movieTag = row.QuerySelector("div")?.QuerySelector("b");
				if (movieTag != null)
				{
					var movieQuality = Regex.Matches(movieTag.InnerText, "\\b\\d{3,4}p\\b").FirstOrDefault()?.ToString();
					var movieTorrent = row.QuerySelector(".safapp")?.GetAttributeValue("href", null);

					if (movieTorrent == null)
						throw new Exception("Incorrect torrent table movie page structure");

					if (!movies.Any(movie => movie.Name == movieName))
						movies.Add(
							new Movie
							{
								Name = movieName ?? "NoName",
								Link = movieTorrent,
								Quality = movieQuality,
								GenreId = genre.ID
							}
						);
				}
			}
		}

		private async Task GetPage(HtmlDocument genrePage, string pageUrl)
		{
			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, pageUrl));

				var html = await response.Content.ReadAsStringAsync();
				genrePage.LoadHtml(html);
			}
		}
	}
}
