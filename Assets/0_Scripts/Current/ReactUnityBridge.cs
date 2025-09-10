using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// Bridges Unity and React variables, tracks stats, credits required for grab, etc
public class ReactUnityBridge : MonoBehaviour
{
    // These variables should be determined and set by React
    [HideInInspector] public int prizesWon;
    [HideInInspector] public int creditsOwned;
    [HideInInspector] public int creditsNeeded;

    [Header("Text")]
    public TextMeshProUGUI prizesWonText;
    public TextMeshProUGUI creditsOwnedText;
    public TextMeshProUGUI creditsNeededText;
    private void Awake() {
        UpdateCreditsNeeded();
        UpdatePrizesWon();
        LoadReactCredits();
        LoadReactPrizesWon();
    }
    #region React Bridge Functions

    // External JS function Unity uses to send data (creditsNeeded) to React
    [DllImport("__Internal")]
    private static extern void UpdateCurrencyFromUnity(string value);

    [DllImport("__Internal")]
    private static extern void UpdatePrizesFromUnity(string value);

    [DllImport("__Internal")]
    private static extern bool TrySpendCurrencyFromUnity(int amount);

    

    // External JS function Unity calls to fetch current credits from React
#if UNITY_WEBGL && !UNITY_EDITOR
[DllImport("__Internal")]
private static extern int GetCurrency();
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
[DllImport("__Internal")]
private static extern int GetPrizesWon();
#endif

    // Pulls current credits from React (via JS) and updates the Unity UI
    public void LoadReactCredits() {
#if UNITY_WEBGL && !UNITY_EDITOR
    creditsOwned = GetCurrency();
#endif
        creditsOwnedText.text = $"Your credits: ${creditsOwned}";
    }

    public void LoadReactPrizesWon() {
#if UNITY_WEBGL && !UNITY_EDITOR
    prizesWon = GetPrizesWon();
#endif
        prizesWonText.text = $"Prizes won: {prizesWon}";
    }

    // Sends Unity's creditsNeeded value to React via JS bridge
    public void SendCreditsNeededToReact() {
        UpdateCurrencyFromUnity(creditsNeeded.ToString());
    }

    // Receives credits (as string) from React, parses it, and updates Unity's internal state
    public void ReceiveCurrencyFromReact(string value) {
        if (int.TryParse(value, out int parsedValue)) {
            creditsOwned = parsedValue;
            Debug.Log($"Credits owned updated from React: {creditsOwned}");
        } else {
            Debug.LogError($"Failed to parse creditsOwned from React: {value}");
        }
    }

    // Receives credits (as string) from React, parses it, and updates Unity's internal state
    public void ReceivePrizesWonFromReact(string value) {
        if (int.TryParse(value, out int parsedValue)) {
            prizesWon = parsedValue;
            Debug.Log($"Credits owned updated from React: {prizesWon}");
        } else {
            Debug.LogError($"Failed to parse prizesWon from React: {value}");
        }
    }
    
    // Sends Unity's prizesWon value to React via JS bridge
    public void SendPrizesWonToReact() {
        UpdatePrizesFromUnity(prizesWon.ToString());
    }
    #endregion


    public void UpdatePrizesWon() {
        prizesWonText.text = $"Prizes won: {prizesWon}";
#if UNITY_WEBGL && !UNITY_EDITOR
    UpdatePrizesFromUnity(prizesWon.ToString());
#endif
    }
    public void UpdateCreditsOwned() {
        creditsOwnedText.text = $"Your credits: ${creditsOwned}";
    }
    public void UpdateCreditsNeeded() {
        creditsNeededText.text = $"Credits to play: ${creditsNeeded}";
    }

    public void AddCredits(int credits) {
        creditsOwned += credits;
        UpdateCreditsOwned();
    }

    public void RemoveCredits(int credits) {
#if UNITY_WEBGL && !UNITY_EDITOR
    bool success = TrySpendCurrencyFromUnity(credits);
    if (success) {
        creditsOwned -= credits;
        UpdateCreditsOwned();
        UpdateCurrencyFromUnity(creditsOwned.ToString());
    } else {
        Debug.LogWarning("Not enough credits to deduct!");
    }
#else
        creditsOwned -= credits;
        UpdateCreditsOwned();
#endif
    }


    public void playGame() {
        if (creditsOwned >= creditsNeeded) {
            RemoveCredits(creditsNeeded);
        }
    }  
}
