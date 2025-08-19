using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorApp.Helpers;

public static class ViteHelper
{
    private static volatile Dictionary<string, ViteManifestEntry>? _manifest; 
    private static readonly object _lock = new();

    public static IHtmlContent ViteAssets(this IHtmlHelper htmlHelper, IWebHostEnvironment env, params string[] entryPoints)
    {
        bool isDevelopment = env.IsDevelopment();
        var content = new HtmlContentBuilder();

        if (isDevelopment)
        {
            // Development mode - use Vite dev server
            content.AppendHtml("<script type=\"module\" src=\"http://localhost:5173/@vite/client\"></script>");
                
            foreach (string entryPoint in entryPoints)
            {
                content.AppendHtml($"<script type=\"module\" src=\"http://localhost:5173/{entryPoint}\"></script>");
            }
        }
        else
        {
            // Production mode - use built assets
            Dictionary<string, ViteManifestEntry> manifest = GetManifest(env);
                
            foreach (string entryPoint in entryPoints)
            {
                if (manifest.TryGetValue(entryPoint, out ViteManifestEntry? entry))
                {
                    // Add main file
                    if (entry.File.EndsWith(".css"))
                    {
                        content.AppendHtml($"<link rel=\"stylesheet\" href=\"/dist/{entry.File}\">");
                    }
                    else if (entry.File.EndsWith(".js"))
                    {
                        content.AppendHtml($"<script type=\"module\" src=\"/dist/{entry.File}\"></script>");
                    }

                    // Add CSS imports
                    if (entry.Css != null)
                    {
                        foreach (string css in entry.Css)
                        {
                            content.AppendHtml($"<link rel=\"stylesheet\" href=\"/dist/{css}\">");
                        }
                    }

                    // Add preload for imports
                    if (entry.Imports != null)
                    {
                        foreach (string import in entry.Imports)
                        {
                            if (manifest.TryGetValue(import, out ViteManifestEntry? importEntry))
                            {
                                content.AppendHtml($"<link rel=\"modulepreload\" href=\"/dist/{importEntry.File}\">");
                            }
                        }
                    }
                }
            }
        }

        return content;
    }

    private static Dictionary<string, ViteManifestEntry> GetManifest(IWebHostEnvironment env)
    {
        if (_manifest != null)
        {
            return _manifest;
        }

        lock (_lock)
        {
            if (_manifest != null)
            {
                return _manifest;
            }

            string manifestPath = Path.Combine(env.WebRootPath, "dist", "manifest.json");
                
            if (!File.Exists(manifestPath))
            {
                throw new FileNotFoundException($"Vite manifest not found at {manifestPath}. Run 'npm run build' first.");
            }

            string json = File.ReadAllText(manifestPath);
            _manifest = JsonSerializer.Deserialize<Dictionary<string, ViteManifestEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new Dictionary<string, ViteManifestEntry>();
        }

        return _manifest;
    }
}

public class ViteManifestEntry
{
    public string File { get; set; } = string.Empty;
    public string[]? Css { get; set; }
    public string[]? Imports { get; set; }
    public bool IsEntry { get; set; }
}

