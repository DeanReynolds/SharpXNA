using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Specialized;

namespace SharpXNA
{
    public class CustomContentManager : ContentManager
    {
        public CustomContentManager(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public CustomContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory) { }

        protected OrderedDictionary assets = new OrderedDictionary(), disposables = new OrderedDictionary();
        protected IDisposable latestDisposable;

        public override T Load<T>(string assetName)
        {
            string key = (typeof(T).Name + "." + assetName);
            if (assets.Contains(key)) return (T)assets[key];
            else
            {
                T asset = ReadAsset<T>(assetName, RecordDisposableAsset);
                assets.Add(key, asset);
                if (latestDisposable != null) { disposables.Add(asset, latestDisposable); latestDisposable = null; }
                return asset;
            }
        }
        public bool Save<T>(string assetName, object asset) { string key = (typeof(T).Name + "." + assetName); if (!assets.Contains(key)) { assets.Add(key, asset); return true; } else return false; }
        public bool Loaded<T>(string assetName) { string key = (typeof(T).Name + "." + assetName); return (assets.Contains(key) && (assets[key] is T)); }
        public override void Unload()
        {
            foreach (IDisposable disposable in disposables) disposable.Dispose();
            assets.Clear();
            disposables.Clear();
        }
        public bool Unload<T>(string assetName)
        {
            string key = (typeof(T).Name + "." + assetName);
            if (assets.Contains(key))
            {
                if (disposables.Contains(assets[key]))
                {
                    (disposables[assets[key]] as IDisposable).Dispose();
                    disposables.Remove(assets[key]);
                }
                assets.Remove(key);
                return true;
            }
            else return false;
        }
        public bool Unload(object asset)
        {
            for (int i = 0; i < assets.Count; i++)
                if (assets[i] == asset)
                {
                    if (disposables.Contains(asset))
                    {
                        (disposables[assets[i]] as IDisposable).Dispose();
                        disposables.Remove(assets[i]);
                    }
                    assets.RemoveAt(i);
                    return true;
                }
            return false;
        }

        internal void RecordDisposableAsset(IDisposable disposable) { latestDisposable = disposable; }
    }
}