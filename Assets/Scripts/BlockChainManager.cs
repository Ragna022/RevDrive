using UnityEngine;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using Solana.Unity.Metaplex;
using Solana.Unity.Metaplex.NFT.Library;
using System.Collections.Generic;
using System.Threading.Tasks;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Metaplex.Utilities;
using Solana.Unity.Rpc.Models;


public class BlockChainManager : MonoBehaviour
{
    public RpcCluster rpcCluster = RpcCluster.DevNet;
    private WalletBase wallet;

    private void Start()
    {
        StartCoroutine(InitializeWallet());
    }

    private System.Collections.IEnumerator InitializeWallet()
    {
        var walletControllerObject = GameObject.Find("WalletController");
        var walletController = walletControllerObject != null ? walletControllerObject.GetComponent<Web3>() : null;

        if (walletController != null)
        {
            yield return new WaitUntil(() => Web3.Wallet != null);
            wallet = Web3.Wallet;

            if (wallet != null)
            {
                Debug.Log("✅ Wallet connected: " + wallet.Account.PublicKey);

                // First, let's check wallet balance
                //StartCoroutine(CheckBalanceAndMint());
            }
            else
            {
                Debug.LogError("❌ Wallet connection failed. Check WalletController setup.");
            }
        }
        else
        {
            Debug.LogError("❌ WalletController not found in scene. Ensure the prefab is active.");
        }
    }

    private System.Collections.IEnumerator CheckBalanceAndMint()
    {
        yield return StartCoroutine(RunTask(CheckBalanceAsync()));
    }

    private async Task CheckBalanceAsync()
    {
        try
        {
            // Check SOL balance first
            var balanceResult = await Web3.Rpc.GetBalanceAsync(Web3.Account.PublicKey);
            if (balanceResult.WasSuccessful)
            {
                var balanceInSOL = (double)balanceResult.Result.Value / 1000000000; // Convert lamports to SOL
                Debug.Log($"💰 Wallet balance: {balanceInSOL:F4} SOL");

                if (balanceInSOL < 0.01) // Need at least 0.01 SOL
                {
                    Debug.LogError("❌ Insufficient SOL balance. You need at least 0.01 SOL for DevNet. Get some from: https://faucet.solana.com/");
                    return;
                }

                // Check if user already has a starter car NFT
                bool hasStarterCar = await CheckForExistingStarterCar();

                if (hasStarterCar)
                {
                    Debug.Log("🎯 You already have a starter car NFT! Skipping minting.");
                    return;
                }

                // If no starter car found, proceed with minting
                await MintNFTSimplifiedAsync();
            }
            else
            {
                Debug.LogError("❌ Failed to check balance: " + balanceResult.Reason);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("💥 Balance check exception: " + ex.Message);
        }
    }

    private async Task<bool> CheckForExistingStarterCar()
    {
        try
        {
            Debug.Log("🔍 Checking for existing starter car NFT...");

            // Instead of scanning all tokens, let's directly check our known NFT addresses
            string[] knownCarNFTs = {
                "DXH7mUyy9UEtEwDpNysrjrV4YPP619g41ekDjshAUiNU", // First NFT
                //"8oUUW74Lvg5epiCBg5KYLVfPrrVQqiwiPMEAaeR69eGK"  // Second NFT
            };

            foreach (string mintAddress in knownCarNFTs)
            {
                // Check if this NFT exists and if we own it
                bool ownsThisNFT = await CheckIfOwnsNFT(mintAddress);
                if (ownsThisNFT)
                {
                    Debug.Log($"✅ Found existing starter car NFT: {mintAddress}");
                    return true;
                }
            }

            Debug.Log("🔍 No existing starter car NFT found. Ready to mint!");
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error checking for existing NFT: {ex.Message}");
            return false; // If we can't check, allow minting to proceed
        }
    }

    private async Task<bool> CheckIfOwnsNFT(string mintAddress)
    {
        try
        {
            // Check if the mint exists first
            var mintAccountResult = await Web3.Rpc.GetAccountInfoAsync(mintAddress);
            if (!mintAccountResult.WasSuccessful || mintAccountResult.Result.Value == null)
            {
                return false; // Mint doesn't exist
            }

            // Get the associated token account for this mint and our wallet
            var associatedTokenAccount = Solana.Unity.Programs.AssociatedTokenAccountProgram
                .DeriveAssociatedTokenAccount(Web3.Account.PublicKey, (PublicKey)mintAddress);

            // Check if we have this token account
            var tokenAccountResult = await Web3.Rpc.GetAccountInfoAsync(associatedTokenAccount);
            if (!tokenAccountResult.WasSuccessful || tokenAccountResult.Result.Value == null)
            {
                return false; // We don't have this token
            }

            // Check the token balance
            var balanceResult = await Web3.Rpc.GetTokenAccountBalanceAsync(associatedTokenAccount);
            if (balanceResult.WasSuccessful &&
                balanceResult.Result.Value.Amount == "1" &&
                balanceResult.Result.Value.Decimals == 0)
            {
                return true; // We own 1 of this NFT
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error checking NFT ownership for {mintAddress}: {ex.Message}");
            return false;
        }
    }

    private async Task MintNFTSimplifiedAsync()
    {
        try
        {
            Debug.Log("🚀 Starting simplified NFT minting...");

            // Step 1: Create mint account
            var mint = new Account();
            Debug.Log("📝 Mint account generated: " + mint.PublicKey);

            // Step 2: Create simplified metadata (shorter to avoid transaction size issues)
            var metadata = new Metadata()
            {
                name = "Car NFT",
                symbol = "CAR",
                uri = "https://cyan-elderly-lobster-29.mypinata.cloud/ipfs/bafkreifnoj4cdsfbp4y55i2xkmsyetjx6fgeqfsesebjpyxqazhwehvrti",
                sellerFeeBasisPoints = 0, // No royalty to simplify
                creators = new List<Creator> { new Creator(Web3.Account.PublicKey, 100, true) }
            };

            Debug.Log("📋 Simplified metadata created");

            // Step 3: Get fresh blockhash and rent
            var blockHashResult = await Web3.Rpc.GetLatestBlockHashAsync();
            var minimumRentResult = await Web3.Rpc.GetMinimumBalanceForRentExemptionAsync(TokenProgram.MintAccountDataSize);

            if (!blockHashResult.WasSuccessful || !minimumRentResult.WasSuccessful)
            {
                Debug.LogError("❌ Failed to get blockchain data");
                return;
            }

            var blockHash = blockHashResult.Result.Value.Blockhash;
            var minimumRent = minimumRentResult.Result;
            var associatedTokenAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(Web3.Account, mint.PublicKey);

            // Step 4: Build transaction in smaller chunks to avoid size issues
            // First transaction: Create mint and token accounts
            Debug.Log("🔨 Building setup transaction...");

            var setupTransaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash)
                .SetFeePayer(Web3.Account)
                .AddInstruction(SystemProgram.CreateAccount(
                    Web3.Account,
                    mint.PublicKey,
                    minimumRent,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMint(
                    mint.PublicKey,
                    0,
                    Web3.Account,
                    Web3.Account))
                .AddInstruction(AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                    Web3.Account,
                    Web3.Account,
                    mint.PublicKey))
                .AddInstruction(TokenProgram.MintTo(
                    mint.PublicKey,
                    associatedTokenAccount,
                    1,
                    Web3.Account));

            // Send setup transaction
            var setupTxBytes = setupTransaction.Build(new List<Account> { Web3.Account, mint });
            var setupTx = Transaction.Deserialize(setupTxBytes);

            Debug.Log("📤 Sending setup transaction...");
            var setupResult = await Web3.Wallet.SignAndSendTransaction(setupTx);

            if (!setupResult.WasSuccessful)
            {
                Debug.LogError("❌ Setup transaction failed: " + setupResult.Reason);
                return;
            }

            Debug.Log("✅ Setup transaction successful: " + setupResult.Result);

            // Wait a bit for the transaction to be confirmed
            await Task.Delay(3000);

            // Step 5: Second transaction for metadata
            Debug.Log("🔨 Building metadata transaction...");

            // Get fresh blockhash for second transaction
            var blockHashResult2 = await Web3.Rpc.GetLatestBlockHashAsync();
            if (!blockHashResult2.WasSuccessful)
            {
                Debug.LogError("❌ Failed to get fresh blockhash for metadata");
                return;
            }

            var metadataTransaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHashResult2.Result.Value.Blockhash)
                .SetFeePayer(Web3.Account)
                .AddInstruction(MetadataProgram.CreateMetadataAccount(
                    PDALookup.FindMetadataPDA(mint),
                    mint.PublicKey,
                    Web3.Account,
                    Web3.Account,
                    Web3.Account.PublicKey,
                    metadata,
                    TokenStandard.NonFungible,
                    true,
                    true,
                    null,
                    metadataVersion: MetadataVersion.V3))
                .AddInstruction(MetadataProgram.CreateMasterEdition(
                    maxSupply: null,
                    masterEditionKey: PDALookup.FindMasterEditionPDA(mint),
                    mintKey: mint,
                    updateAuthorityKey: Web3.Account,
                    mintAuthority: Web3.Account,
                    payer: Web3.Account,
                    metadataKey: PDALookup.FindMetadataPDA(mint),
                    version: CreateMasterEditionVersion.V3));

            // Send metadata transaction
            var metadataTxBytes = metadataTransaction.Build(new List<Account> { Web3.Account });
            var metadataTx = Transaction.Deserialize(metadataTxBytes);

            Debug.Log("📤 Sending metadata transaction...");
            var metadataResult = await Web3.Wallet.SignAndSendTransaction(metadataTx);

            if (metadataResult.WasSuccessful)
            {
                Debug.Log("🎉 NFT minted successfully!");
                Debug.Log("📍 Setup transaction: " + setupResult.Result);
                Debug.Log("📍 Metadata transaction: " + metadataResult.Result);
                Debug.Log("🔍 View setup tx: https://explorer.solana.com/tx/" + setupResult.Result + "?cluster=devnet");
                Debug.Log("🔍 View metadata tx: https://explorer.solana.com/tx/" + metadataResult.Result + "?cluster=devnet");
                Debug.Log("🏆 NFT Mint Address: " + mint.PublicKey);
                Debug.Log("🔍 View NFT: https://explorer.solana.com/address/" + mint.PublicKey + "?cluster=devnet");
            }
            else
            {
                Debug.LogError("❌ Metadata transaction failed: " + metadataResult.Reason);
                Debug.Log("ℹ️  Token was created but metadata failed. Mint address: " + mint.PublicKey);
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError("💥 Simplified minting exception: " + ex.Message + "\nStack: " + ex.StackTrace);
        }
    }

    private System.Collections.IEnumerator RunTask(Task task)
    {
        var timeout = 60f;
        var elapsed = 0f;

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
}
