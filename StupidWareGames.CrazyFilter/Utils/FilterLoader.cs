namespace StupidWareGames.CrazyFilter.Utils;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Media.Imaging;
using StupidWareGames.CrazyFilter.Interfaces;

public class FilterLoader
{
    private readonly string _assetsRoot;

    public FilterLoader(string assetsRoot)
    {
        _assetsRoot = assetsRoot;
    }

    public List<FilterAsset> LoadAllFilters()
    {
        var filters = new List<FilterAsset>();

        var bodyPartTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IBodyPart).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToDictionary(t => t.Name.Replace("Part", ""), t => t);

        if (!Directory.Exists(_assetsRoot))
            return filters;

        foreach (var partDir in Directory.GetDirectories(_assetsRoot))
        {
            var partName = Path.GetFileName(partDir);
            if (!bodyPartTypes.TryGetValue(partName, out var type))
                continue;

            if (!(Activator.CreateInstance(type) is IBodyPart bodyPart))
                continue;

            foreach (var file in Directory.GetFiles(partDir, "*.png"))
            {
                try
                {
                    var asset = new FilterAsset
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        FilePath = file,
                        TargetPart = bodyPart,
                        Image = new Bitmap(file)
                    };

                    filters.Add(asset);
                }
                catch
                {
                    // Could log if desired
                }
            }
        }

        return filters;
    }
}
