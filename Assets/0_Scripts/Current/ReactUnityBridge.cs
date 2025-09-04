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
    }

    

    public void LoadReactCredits() {
#if UNITY_WEBGL && !UNITY_EDITOR
        creditsOwned = GetCurrency();

#endif
        creditsOwnedText.text = $"Your credits: ${creditsOwned}";
    }
    public void UpdatePrizesWon() {
        prizesWonText.text = $"Prizes won: {prizesWon}";
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
        creditsOwned -= credits;
        UpdateCreditsOwned();
#if UNITY_WEBGL && !UNITY_EDITOR
        UpdateCurrencyFromUnity(creditsOwned.ToString());
#endif
    }


    public void playGame() {
        if (creditsOwned >= creditsNeeded) {
            RemoveCredits(creditsNeeded);
        }
    }



    [DllImport("__Internal")]
    private static extern void UpdateCurrencyFromUnity(string value);
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int GetCurrency();
#endif
    // Call this when creditsNeeded changes to notify React
    public void SendCreditsNeededToReact() {
        UpdateCurrencyFromUnity(creditsNeeded.ToString());
    }

    // Called from React via sendMessage
    public void ReceiveCurrencyFromReact(string value) {
        if (int.TryParse(value, out int parsedValue)) {
            creditsOwned = parsedValue;
            Debug.Log($"Credits owned updated from React: {creditsOwned}");
        } else {
            Debug.LogError($"Failed to parse creditsOwned from React: {value}");
        }
    }
}
