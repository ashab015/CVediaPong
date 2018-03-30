using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Loads a texture from a asset bundle from the specified url. </summary>
public class AssetBundleDownloader : MonoBehaviour {

    /// <summary> True to done. </summary>
    public bool Done = false;
    /// <summary> The texture background. </summary>
    public UITexture TextureBackground;
    /// <summary> URL of the document. </summary>
    public string URL;

    /// <summary> Load the texture when the game starts. </summary>
    private void Start()
    {
        StartCoroutine(StartDownload());
    }

    /// <summary>
    /// Uses WWW and the load asset functions to download the asset and extract the texture and apply it
    /// to the UITexture.
    /// </summary>
    ///
    /// <returns> An IEnumerator. </returns>
    IEnumerator StartDownload()
    {
        while (!Caching.ready)
            yield return null;
        // Start a download of the given URL
        WWW www = WWW.LoadFromCacheOrDownload(URL, 1);
        // Wait for download to complete
        yield return www;
        // Load and retrieve the AssetBundle
        AssetBundle bundle = www.assetBundle;
        // Load the object asynchronously
        AssetBundleRequest request = bundle.LoadAssetAsync("SantaAssetBundle", typeof(Texture));
        // Wait for completion
        yield return request;
        // Get the reference to the loaded object
        Texture obj = request.asset as Texture;
        TextureBackground.mainTexture = obj;
        // Unload the AssetBundles compressed contents to conserve memory
        bundle.Unload(false);
        // Frees the memory from the web stream
        www.Dispose();
        Done = true;
    }



}
