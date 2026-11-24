using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("Aquarium Currency")]
    [SerializeField] private int startingBalance = 0;

    public int Balance { get; private set; }

    public int TotalDonationsReceived { get; private set; }

    private void Awake()
    {
        // Make sure only one CurrencyManager exists.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep the currency manager between scenes.
        DontDestroyOnLoad(gameObject);

        Balance = startingBalance;
    }

    public void ReceiveDonation(int amount)
    {
        // Prevent invalid donations.
        if (amount <= 0)
        {
            return;
        }

        // Add the donation to the aquarium's money.
        Balance += amount;
        TotalDonationsReceived += amount;

        Debug.Log("Visitor donated " + amount + " coins!");

        Debug.Log("Aquarium balance: " + Balance);
    }
}