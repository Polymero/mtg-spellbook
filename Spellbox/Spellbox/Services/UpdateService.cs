using System.Diagnostics;

namespace Spellbox.Services
{
    public sealed record VersionInfo(
        string Version,
        string Rid
    );

    public sealed class UpdateService
    {
        private const string Repo = "Polymero/Spellbox";



        public async Task DownloadAndUpdateAsync(VersionInfo version)
        {
            var zipPath = Path.Combine(Path.GetTempPath(), $"Spellbox-{version.Version}-{version.Rid}");
        }

    }

    public static class AppPaths
    {
        public static string AppDir => Path.GetDirectoryName(
            Environment.ProcessPath
        )!;

        public static string AppData => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Spellbox"
        );

        public static string OracleDb => Path.Combine(AppData, "OracleCards.db3");
        public static string CollectionDb => Path.Combine(AppData, "Collection.db3");
        public static string CardMarketDb => Path.Combine(AppData, "CardMarketPricing.db3");
    }
}
