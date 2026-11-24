using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [Header("Currency Display")]
    [SerializeField] private TMP_Text currencyText;

    private int lastDisplayedBalance = -1;

    private void Start()
    {
        if (currencyText != null)
        {
            currencyText.text = "0";
        }
    }

    private void Update()
    {
        if (CurrencyManager.Instance == null || currencyText == null)
        {
            return;
        }

        int currentBalance = CurrencyManager.Instance.Balance;

        // Only update the text when the balance changes.
        if (currentBalance != lastDisplayedBalance)
        {
            currencyText.text = currentBalance.ToString();

            lastDisplayedBalance = currentBalance;
        }
    }
}