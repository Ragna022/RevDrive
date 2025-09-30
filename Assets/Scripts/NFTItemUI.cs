using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class NFTItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nftNameText;
    //public TMP_Text nftRarityText;   // New rarity field
    public Image nftImageDisplay;
    public Button viewButton;

    private SimpleCarNFT nftData;
    private Action<string> onViewCallback;
    private Coroutine imageLoadCoroutine;
    private bool isDestroyed = false;

    public void Initialize(SimpleCarNFT nft, Action<string> viewCallback = null)
    {
        if (isDestroyed) return;
        
        nftData = nft;
        onViewCallback = viewCallback;

        if (nftNameText != null) nftNameText.text = nft.name;
        //if (nftRarityText != null) nftRarityText.text = $"Rarity: {nft.rarity}"; // Show rarity
        if (nftImageDisplay != null) nftImageDisplay.color = Color.clear;

        if (viewButton != null && onViewCallback != null)
        {
            viewButton.onClick.RemoveAllListeners();
            viewButton.onClick.AddListener(OnViewButtonClicked);
        }
    }

    void OnEnable()
    {
        if (isDestroyed || nftData == null) return;
        
        if (imageLoadCoroutine != null)
        {
            StopCoroutine(imageLoadCoroutine);
        }
        imageLoadCoroutine = StartCoroutine(LoadNFTImage());
    }

    void OnDisable()
    {
        if (imageLoadCoroutine != null)
        {
            StopCoroutine(imageLoadCoroutine);
            imageLoadCoroutine = null;
        }
    }

    private IEnumerator LoadNFTImage()
    {
        if (isDestroyed || nftData == null) yield break;

        float timeout = 10f;
        float elapsed = 0f;

        while (nftData.loadedImage == null && elapsed < timeout && !isDestroyed)
        {
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (isDestroyed) yield break;

        if (nftData.loadedImage != null)
        {
            DisplayImage(nftData.loadedImage);
        }
        else if (!isDestroyed)
        {
            imageLoadCoroutine = StartCoroutine(LoadImageDirectly(nftData.imageUrl));
        }
    }

    private IEnumerator LoadImageDirectly(string imageUrl)
    {
        if (isDestroyed) yield break;

        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();
            
            if (isDestroyed) yield break;
            
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Texture2D texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
                DisplayImage(texture);
            }
            else if (!isDestroyed && nftImageDisplay != null)
            {
                nftImageDisplay.color = Color.gray;
            }
        }
    }

    private void DisplayImage(Texture2D texture)
    {
        if (isDestroyed || nftImageDisplay == null || texture == null) return;

        Sprite nftSprite = Sprite.Create(
            texture, 
            new Rect(0, 0, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f),
            100f
        );
        
        nftImageDisplay.sprite = nftSprite;
        nftImageDisplay.color = Color.white;
        nftImageDisplay.preserveAspect = true;
    }

    private void OnViewButtonClicked()
    {
        if (isDestroyed) return;
        onViewCallback?.Invoke(nftData.mintAddress);
    }

    void OnDestroy()
    {
        isDestroyed = true;
        if (imageLoadCoroutine != null)
        {
            StopCoroutine(imageLoadCoroutine);
        }
    }
}
