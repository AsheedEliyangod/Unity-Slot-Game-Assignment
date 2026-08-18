using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("Coin Settings")]
    public int startingCoins = 0;

    [Header("UI")]
    public TMP_Text coinText;

    private int coins;

    public int Coins => coins;


    private void Start()
    {
        coins = startingCoins;

        UpdateCoinUI();
    }


    // =========================================================
    // ADD COINS
    // =========================================================

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        coins += amount;

        UpdateCoinUI();

        Debug.Log(
            "Coins added: +" +
            amount +
            " | Total: " +
            coins
        );
    }


    // =========================================================
    // REMOVE COINS
    // =========================================================

    public bool RemoveCoins(int amount)
    {
        if (amount <= 0)
            return true;

        if (coins < amount)
        {
            Debug.Log(
                "Not enough coins! Current: " +
                coins +
                " | Required: " +
                amount
            );

            return false;
        }

        coins -= amount;

        UpdateCoinUI();

        Debug.Log(
            "Coins removed: -" +
            amount +
            " | Total: " +
            coins
        );

        return true;
    }


    // =========================================================
    // UPDATE UI
    // =========================================================

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }
}