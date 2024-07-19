using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MystMovie.DB
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddPersistence(this IServiceCollection
			services, string connectionString)
		{
			if (services is null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentException($"\"{nameof(connectionString)}\" не может быть неопределенным или пустым.", nameof(connectionString));
			}

			services.AddDbContext<MovieContext>(options =>
			{
				options.UseNpgsql(
					connectionString,
					options => options.EnableRetryOnFailure(
					maxRetryCount: 5,
					maxRetryDelay: System.TimeSpan.FromSeconds(30),
					errorCodesToAdd: null)
				);
			});

			services.AddScoped<MovieContext>();

			return services;
		}
	}
}
