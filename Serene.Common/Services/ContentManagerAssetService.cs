using System;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework.Content;
using Serene.Common.Interfaces;

namespace Serene.Common.Services;

public class AssetService : IAssetService, IAssetBootstrapper
{
    private ContentManager? _content;
    private readonly ConcurrentDictionary<(Type type, string name), object> _cache = new();
    public bool IsInitialized => _content != null;

    public void ClearCache()
    {
        _cache.Clear();
    }

    public void Initialize(ContentManager content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));
        if (_content != null) return; // idempotent: ignore repeated init

        _content = content;
    }

    public T Load<T>(string assetName) where T : class
    {
        if (string.IsNullOrWhiteSpace(assetName))
            throw new ArgumentException("Asset name must not be empty.", nameof(assetName));

        var content = _content ?? throw new InvalidOperationException(
            "AssetService is not initialized yet. Call IAssetBootstrapper.Initialize(Game.Content) first.");

        var key = (typeof(T), assetName);

        var obj = _cache.GetOrAdd(key, _ =>
        {
            var loaded = content.Load<T>(assetName);
            return loaded is null
                ? throw new ContentLoadException($"MonoGame Content returned null for '{assetName}' ({typeof(T).Name}).")
                : (object)loaded;
        });

        return (T)obj;
    }

    public bool TryGet<T>(string assetName, out T? asset) where T : class
    {
        asset = null;
        if (string.IsNullOrWhiteSpace(assetName)) return false;

        var key = (typeof(T), assetName);
        if (_cache.TryGetValue(key, out var obj))
        {
            asset = (T)obj;
            return true;
        }
        return false;
    }
}
