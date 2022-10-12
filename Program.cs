using System.Reflection;
using System.ServiceModel.Syndication;
using System.Xml;
using FuzzySharp;

namespace TeleprompterConsole;

internal class Program
{
    static void Main(string[] args)
{
    if (args.Length == 0)
    {
        var versionString = Assembly.GetEntryAssembly()?
                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                .InformationalVersion
                                .ToString();
        var repoUrl = "https://github.com/Starr-Labs/dotnet-rocks.git";

        Console.WriteLine($"rocks v{versionString} details at {repoUrl}\n");
        Console.WriteLine("-------------");
        Console.WriteLine("\nUsage:");
        Console.WriteLine("  rocks <query>");
        return;
    }

    SearchQuickly(string.Join(' ', args));
}
    private static IEnumerable<SyndicationItem> GetFeedItems()
    {
        var url = "https://www.spreaker.com/show/5634793/episodes/feed?show=dotnetrocks";
        using var reader = XmlReader.Create(url);
        var feed = SyndicationFeed.Load(reader).Items;
        return feed;
    }

    private static void SearchQuickly(string v)
    {
        var searchRatio = 40;
        var feed = GetFeedItems();
        var results = new List<SyndicationItem>();
        
        foreach (var post in feed)
        {
            var searchTerm = post.Title + " " + post.Summary + post.PublishDate.ToString("D");
            if (Fuzz.PartialRatio(searchTerm, v) >= searchRatio)
            {
                results.Add(post);
            }
        }

        if (results.Count() == 0)
        {
            Console.WriteLine("Sorry, I got nothing. Keep rocking.");
            return;
        }

        Console.WriteLine($"Top 10 of the {results.Count} results for: \"{v}\"{Environment.NewLine}");
        foreach(var post in results.Take(10))
        {
            var summaryFirstSentence = post.Summary.Text.Split(".")[0];
            Console.WriteLine(
                $"\t{post.Title.Text} on {post.PublishDate.ToString("d")} on {Environment.NewLine}\t{summaryFirstSentence} at: {Environment.NewLine}\t{post.Links[0].GetAbsoluteUri().ToString()}.{Environment.NewLine}"); 
        }
        Console.WriteLine($"Top 10 of the {results.Count} results for: \"{v}\"{Environment.NewLine}");
    }
}
