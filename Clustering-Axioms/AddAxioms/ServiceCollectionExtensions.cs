using Microsoft.Extensions.DependencyInjection;

namespace AddAxioms;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAxiomFileDataSource(this IServiceCollection services, string filePath)
		=> services.AddSingleton<IReadAxioms, ReadRepo>(r => new ReadRepo(filePath));

}
