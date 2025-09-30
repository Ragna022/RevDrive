using UnityEngine;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

[Serializable]
public class SimpleCarNFT
{
    public string mintAddress;
    public string name;
    public string description;
    public string imageUrl;
    public Texture2D loadedImage;
}

public class SimpleNFTManager : MonoBehaviour
{
    [Header("NFT Detection")]
    public List<SimpleCarNFT> detectedNFTs = new List<SimpleCarNFT>();
    
    private const string ACTUAL_NFT_IMAGE_URL = "https://cyan-elderly-lobster-29.mypinata.cloud/ipfs/bafkreiann47qsp2fdrag46jdxhqz7jyjt23mtb4jhd47dk3tv5xq6drse4";
    
    private WalletBase wallet;

    void Start()
    {
        StartCoroutine(InitializeNFTDetection());
    }

    private System.Collections.IEnumerator InitializeNFTDetection()
    {
        yield return new WaitUntil(() => Web3.Wallet != null);
        wallet = Web3.Wallet;

        if (wallet != null)
        {
            Debug.Log("Starting NFT detection for wallet: " + wallet.Account.PublicKey);
            yield return StartCoroutine(RunTask(DetectMintedNFT()));
        }
        else
        {
            Debug.LogError("Wallet not available for NFT detection");
        }
    }

    public async Task DetectMintedNFT()
    {
        try
        {
            // SAFETY CHECK: Are we still valid?
            if (!AsyncSafety.IsValid(this)) 
            {
                Debug.Log("NFT detection cancelled - object destroyed");
                return;
            }

            Debug.Log("Looking for minted NFT...");

            string[] mintAddresses = {
                "DXH7mUyy9UEtEwDpNysrjrV4YPP619g41ekDjshAUiNU"
            };

            foreach (string mintAddress in mintAddresses)
            {
                // SAFETY CHECK: Still valid during loop?
                if (!AsyncSafety.IsValid(this)) 
                {
                    Debug.Log("NFT detection interrupted - object destroyed");
                    return;
                }

                var accountResult = await Web3.Rpc.GetAccountInfoAsync(mintAddress);

                if (accountResult.WasSuccessful && accountResult.Result.Value != null)
                {
                    Debug.Log($"Found minted NFT: {mintAddress}");

                    var nft = new SimpleCarNFT
                    {
                        mintAddress = mintAddress,
                        name = $"Racing Car NFT ({mintAddress.Substring(0, 8)})",
                        description = "A powerful starter car for racing adventures",
                        imageUrl = ACTUAL_NFT_IMAGE_URL
                    };

                    detectedNFTs.Add(nft);
                    await LoadNFTImage(nft);
                    Debug.Log($"Successfully loaded NFT: {nft.name}");
                }
            }
        }
        catch (System.Exception ex)
        {
            // Only log if we still exist
            if (AsyncSafety.IsValid(this))
            {
                Debug.LogError($"Error detecting NFT: {ex.Message}");
            }
        }
    }

    private async Task LoadNFTImage(SimpleCarNFT nft)
    {
        try
        {
            // SAFETY CHECK: Are we still valid?
            if (!AsyncSafety.IsValid(this)) 
            {
                Debug.Log("Image loading cancelled - object destroyed");
                return;
            }

            string[] imageUrls = {
                nft.imageUrl,
                "https://picsum.photos/256/256",
                "https://via.placeholder.com/256x256/FF6B6B/FFFFFF?text=CAR+NFT"
            };

            foreach (string imageUrl in imageUrls)
            {
                // SAFETY CHECK: Still valid during image loading?
                if (!AsyncSafety.IsValid(this)) 
                {
                    Debug.Log("Image loading interrupted - object destroyed");
                    return;
                }

                using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(imageUrl))
                {
                    www.timeout = 30;
                    var operation = www.SendWebRequest();
                    float timeout = 35f;
                    float elapsed = 0f;

                    while (!operation.isDone && elapsed < timeout)
                    {
                        // SAFETY CHECK: Still valid during wait?
                        if (!AsyncSafety.IsValid(this)) 
                        {
                            Debug.Log("Image loading interrupted during wait");
                            return;
                        }
                        
                        await AsyncSafety.SafeDelay(0.1f, this);
                        elapsed += 0.1f;
                    }

                    if (www.isDone && www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        nft.loadedImage = UnityEngine.Networking.DownloadHandlerTexture.GetContent(www);
                        Debug.Log($"✅ Successfully loaded image from: {imageUrl}");
                        return;
                    }
                    else
                    {
                        Debug.LogWarning($"❌ Failed to load from {imageUrl}: {www.error}");
                    }
                }
            }

            Debug.LogWarning("All image URLs failed, creating default texture");
            nft.loadedImage = CreateDefaultTexture();
        }
        catch (System.Exception ex)
        {
            // Only log if we still exist
            if (AsyncSafety.IsValid(this))
            {
                Debug.LogError($"💥 Critical error loading NFT image: {ex.Message}");
            }
        }
    }

    private Texture2D CreateDefaultTexture()
    {
        try
        {
            Texture2D texture = new Texture2D(256, 256);
            Color bgColor = new Color(0.1f, 0.3f, 0.6f, 1f);
            
            Color[] pixels = new Color[256 * 256];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = bgColor;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            return texture;
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to create default texture: {ex.Message}");
            Texture2D fallback = new Texture2D(1, 1);
            fallback.SetPixel(0, 0, Color.magenta);
            fallback.Apply();
            return fallback;
        }
    }

    public List<SimpleCarNFT> GetDetectedNFTs()
    {
        return detectedNFTs;
    }

    private System.Collections.IEnumerator RunTask(Task task)
    {
        float timeout = 60f;
        float elapsed = 0f;

        while (!task.IsCompleted && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!task.IsCompleted)
        {
            Debug.LogError("⏰ Task timed out after 60 seconds.");
        }
        else if (task.IsFaulted)
        {
            Debug.LogError("💥 Task failed: " + task.Exception);
        }
    }

    public void StartNFTDetection()
    {
        StartCoroutine(InitializeNFTDetection());
    }
}
