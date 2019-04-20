using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;

namespace RoslynTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			MSBuildLocator.RegisterDefaults();

			var workspace = MSBuildWorkspace.Create();
			var project = await workspace.OpenProjectAsync(@"..\..\..\..\NHibernate.Caches.StackExchangeRedis\NHibernate.Caches.StackExchangeRedis.csproj");
			var compilation = await project.GetCompilationAsync();

			// ICache is an interface from NHibernate assembly, contains Get method definition
			var cacheSymbol = compilation.References
				.Select(compilation.GetAssemblyOrModuleSymbol)
				.OfType<IAssemblySymbol>()
				.Where(o => o.Name == "NHibernate")
				.Select(o => o.GetTypeByMetadataName("NHibernate.Cache.ICache"))
				.First(o => o != null);
			// CacheBase is an abstract class from NHibernate assembly, defines Get method as abstract
			var cacheBaseSymbol = compilation.References
				.Select(compilation.GetAssemblyOrModuleSymbol)
				.OfType<IAssemblySymbol>()
				.Where(o => o.Name == "NHibernate")
				.Select(o => o.GetTypeByMetadataName("NHibernate.Cache.CacheBase"))
				.First(o => o != null);
			// FakeCache is a derived class from CacheBase contained in NHibernate assembly, overrides Get method
			var fakeCacheSymbol = compilation.References
				.Select(compilation.GetAssemblyOrModuleSymbol)
				.OfType<IAssemblySymbol>()
				.Where(o => o.Name == "NHibernate")
				.Select(o => o.GetTypeByMetadataName("NHibernate.Cache.FakeCache"))
				.First(o => o != null);
			// HashtableCache is a derived class from CacheBase contained in NHibernate assembly, overrides Get method
			var hashtableCacheSymbol = compilation.References
				.Select(compilation.GetAssemblyOrModuleSymbol)
				.OfType<IAssemblySymbol>()
				.Where(o => o.Name == "NHibernate")
				.Select(o => o.GetTypeByMetadataName("NHibernate.Cache.HashtableCache"))
				.First(o => o != null);
			// RedisCache is a derived class from CacheBase contained in NHibernate.Caches.StackExchangeRedis assembly, overrides Get method
			var redisCacheSymbol = compilation.GetTypeByMetadataName("NHibernate.Caches.StackExchangeRedis.RedisCache");

			await CheckResult(cacheSymbol.GetMembers("Get").OfType<IMethodSymbol>().First(), project);
			await CheckResult(cacheBaseSymbol.GetMembers("Get").OfType<IMethodSymbol>().First(), project);
			await CheckResult(fakeCacheSymbol.GetMembers("Get").OfType<IMethodSymbol>().First(), project);
			await CheckResult(hashtableCacheSymbol.GetMembers("Get").OfType<IMethodSymbol>().First(), project);
			await CheckResult(redisCacheSymbol.GetMembers("Get").OfType<IMethodSymbol>().First(), project);

			Console.ReadLine();
		}

		private static async Task CheckResult(IMethodSymbol searchMethodSymbol, Project project)
		{
			Console.WriteLine($"FindReferencesAsync for method: {searchMethodSymbol} from assembly: {searchMethodSymbol.ContainingAssembly.Name}");
			var references = await SymbolFinder.FindReferencesAsync(searchMethodSymbol, project.Solution);
			foreach (var reference in references.OrderBy(o => o.Definition.ToString()))
			{
				Console.WriteLine($"Reference definition: {reference.Definition}");
			}

			Console.WriteLine("---------------------------------------------------------------------------------------------");
		}
	}
}
