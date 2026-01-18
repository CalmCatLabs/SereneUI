using Microsoft.Xna.Framework.Content;

namespace Serene.Common.Interfaces;

public interface IAssetService
{
    T Load<T>(string assetName) where T : class;

    bool TryGet<T>(string assetName, out T? asset) where T : class;

    /// <summary>Clears only the cache dictionary, does NOT unload the ContentManager.</summary>
    void ClearCache();
}

public interface IAssetBootstrapper
{
    /// <summary>Provide the ContentManager once (e.g. Game.Content).</summary>
    void Initialize(ContentManager content);
    bool IsInitialized { get; }
}