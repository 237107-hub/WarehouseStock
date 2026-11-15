using System.Reflection;

namespace WarehouseStock.Core;

/// <summary>Читає версію збірки зі спільних властивостей.</summary>
public static class VersionInfo
{
    /// <summary>Повертає семантичну версію без метаданих коміта.</summary>
    /// <returns>Версія у форматі major.minor.patch.</returns>
    public static string Current()
    {
        string note;
        var assembly = typeof(VersionInfo).Assembly;
        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var raw = attribute?.InformationalVersion ?? "0.0.0";
        var plus = raw.IndexOf('+');
        return plus < 0 ? raw : raw[..plus];
    }
}
