using System.Threading.Tasks;
using Solana.Unity.SDK;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

public class SimpleWalletUI : MonoBehaviour
{
    [Header("Wallet Display")]
    public TextMeshProUGUI walletAddressText;
    public GameObject nftPanel;
    public Transform nftContainer;
    public GameObject nftItemPrefab;
    
    private SimpleNFTManager nftManager;
    private bool nftDisplayed = false;

    void Start()
    {
        nftManager = FindObjectOfType<SimpleNFTManager>();
        InitializeUI();
        
        if (Web3.Wallet != null)
        {
            UpdateWalletDisplay();
        }
        
        InvokeRepeating(nameof(CheckForNFTs), 1f, 1f);
    }
    
    private void InitializeUI()
    {
        if (nftPanel != null) nftPanel.SetActive(false);
        
        if (nftContainer != null)
        {
            foreach (Transform child in nftContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    private void UpdateWalletDisplay()
    {
        if (walletAddressText != null && Web3.Wallet != null)
        {
            string address = Web3.Wallet.Account.PublicKey.ToString();
            walletAddressText.text = $"Wallet: {address.Substring(0, 8)}...{address.Substring(address.Length - 8)}";
        }
    }
    
    private void CheckForNFTs()
    {
        if (nftManager != null && nftManager.GetDetectedNFTs().Count > 0 && !nftDisplayed)
        {
            CancelInvoke(nameof(CheckForNFTs));
            DisplayNFTs();
        }
    }
    
    private void DisplayNFTs()
    {
        var nfts = nftManager.GetDetectedNFTs();
        if (nfts.Count > 0 && !nftDisplayed && nftItemPrefab != null && nftContainer != null)
        {
            if (nftPanel != null) nftPanel.SetActive(true);
            
            foreach (Transform child in nftContainer)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var nft in nfts)
            {
                GameObject nftItem = Instantiate(nftItemPrefab, nftContainer);
                nftItem.SetActive(true);
                
                NFTItemUI nftItemUI = nftItem.GetComponent<NFTItemUI>();
                
                if (nftItemUI != null)
                {
                    nftItemUI.Initialize(nft, OnViewNFT);
                }
            }
            
            nftDisplayed = true;
        }
    }
    
    private void OnViewNFT(string mintAddress)
    {
        string explorerUrl = $"https://explorer.solana.com/address/{mintAddress}?cluster=devnet";
        Application.OpenURL(explorerUrl);
    }
    
    public void RefreshNFTs()
    {
        nftDisplayed = false;
        InitializeUI();
        
        if (nftManager != null)
        {
            nftManager.GetDetectedNFTs().Clear();
            nftManager.StartNFTDetection();
        }
        
        InvokeRepeating(nameof(CheckForNFTs), 1f, 1f);
    }
}
