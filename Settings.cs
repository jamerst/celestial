using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;

using Celestial.Triggers;

namespace Celestial;

public class Settings
{
    public IEnumerable<Trigger> Triggers { get; set; } = Enumerable.Empty<Trigger>();

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public static async Task<Settings> LoadFromFileAsync(string path, ILogger logger)
    {
        JsonSerializerOptions options = GetJsonOptions();

        if (File.Exists(path))
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                Settings? read = await JsonSerializer.DeserializeAsync<Settings>(fs, options);

                if (read != null)
                {
                    logger.LogInformation("Successfully loaded settings from {path}", path);
                    return read;
                }
            }
        }

        logger.LogInformation("Generating new config file with defaults at {path}", path);

        FileInfo info = new FileInfo(path);
        if (info.Directory != null)
        {
            logger.LogInformation("Config directory {directory} does not exist, creating", info.DirectoryName);
            info.Directory.Create();
        }

        Settings settings = new Settings();
        using (var fs = new FileStream(path, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync(fs, settings, options);
        }

        return settings;
    }

    private static JsonSerializerOptions GetJsonOptions()
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}