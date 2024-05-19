namespace MystMovie.DB
{

	public class MoviesInfo
	{
		public Doc[] docs { get; set; }
	}

	public class Doc
	{
		public int id { get; set; }
		public string name { get; set; }
		public string alternativeName { get; set; }
		public string enName { get; set; }
		public string type { get; set; }
		public int year { get; set; }
		public string description { get; set; }
		public string shortDescription { get; set; }
		public int movieLength { get; set; }
		public bool isSeries { get; set; }
		public bool ticketsOnSale { get; set; }
		public object totalSeriesLength { get; set; }
		public object seriesLength { get; set; }
		public string ratingMpaa { get; set; }
		public int? ageRating { get; set; }
		public object top10 { get; set; }
		public int? top250 { get; set; }
		public int typeNumber { get; set; }
		public object status { get; set; }
		public Name[] names { get; set; }
		public Externalid externalId { get; set; }
		public object logo { get; set; }
		public Poster poster { get; set; }
		public Backdrop backdrop { get; set; }
		public Rating rating { get; set; }
		public Votes votes { get; set; }
		public Genre[] genres { get; set; }
		public Country[] countries { get; set; }
		public object[] releaseYears { get; set; }
	}

	public class Externalid
	{
		public string kpHD { get; set; }
	}

	public class Poster
	{
		public string url { get; set; }
		public string previewUrl { get; set; }
	}

	public class Backdrop
	{
		public string url { get; set; }
		public string previewUrl { get; set; }
	}

	public class Rating
	{
		public float kp { get; set; }
		public float imdb { get; set; }
		public float filmCritics { get; set; }
		public float russianFilmCritics { get; set; }
	}

	public class Votes
	{
		public int kp { get; set; }
		public int imdb { get; set; }
		public int filmCritics { get; set; }
		public int russianFilmCritics { get; set; }
		public int await { get; set; }
	}

	public class Name
	{
		public string name { get; set; }
		public string language { get; set; }
		public string type { get; set; }
	}

	public class Country
	{
		public string name { get; set; }
	}

}
