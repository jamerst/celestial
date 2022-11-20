using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

using Celestial.Converters;
using Celestial.Triggers;

namespace Celestial;

public class Settings
{
    public IEnumerable<Trigger> Triggers { get; set; } = Enumerable.Empty<Trigger>();

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }


    public static async Task<Settings> LoadFromFileAsync()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "celestial",
            "config.json"
        );

        JsonSerializerOptions options = GetJsonOptions();

        if (File.Exists(path))
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                Settings? read = await JsonSerializer.DeserializeAsync<Settings>(fs, options);

                if (read != null)
                {
                    return read;
                }
            }
        }

        Console.WriteLine($@"Generating new config file with default options: ""{path}""");
        FileInfo info = new FileInfo(path);
        if (info.Directory != null)
        {
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

        options.Converters.Add(new TriggerConverter());
        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}