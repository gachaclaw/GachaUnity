using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance { get; private set; }
    public int creditsOwned;
    public int creditsNeeded;
    public TextMeshProUGUI creditsOwnedText;
    public TextMeshProUGUI creditsNeededText;
    public Animator animator;
    public Button playButton;
    public GameObject clawMachine;
    public GameObject coinIcon;
    public GameObject spawnLocation;
    public GameObject rewardPanel;
    public GameObject spawnContainer;
    public Button closePanel;
    private GameObject spawnedPrize;
    public float chanceToWin = 0.80f;
    private bool isAnimating = false;
    private bool didWin;
    public List<GameObject> prizeObjects = new List<GameObject>();
    public Camera mainCamera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake() {
        UpdateCreditsNeeded();
        LoadReactCredits();
        rewardPanel.SetActive(false);
    }
    void Start()
    {
        QualitySettings.vSyncCount = 0;           // Disable VSync
        Application.targetFrameRate = 60;         // Cap at 60 FPS
        //set creditsOwned = whatever react has
    }

    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ClosePanel();
        }
    }

    public void ClosePanel() {
        rewardPanel.SetActive(false);

        if (spawnedPrize != null) {
            Destroy(spawnedPrize);
            spawnedPrize = null; // clear reference
        }
    }
    public void UpdateCredits() {
        creditsOwnedText.text = $"Your credits: ${creditsOwned}";
    }

    public void LoadReactCredits() {
#if UNITY_WEBGL && !UNITY_EDITOR
        creditsOwned = GetCurrency();

#endif
        creditsOwnedText.text = $"Your credits: ${creditsOwned}";
    }
    public void UpdateCreditsNeeded() {
        creditsNeededText.text = $"Credits to play: ${creditsNeeded}";
    }

    public void AddCredits(int credits) {
        creditsOwned += credits;
        UpdateCredits();
        coinIcon.GetComponent<PopAnimation>().PlayPop();
    }

    public void RemoveCredits(int credits) {
        creditsOwned -= credits;
        UpdateCredits();
        coinIcon.GetComponent<PopAnimation>().PlayPop();
        #if UNITY_WEBGL && !UNITY_EDITOR
            UpdateCurrencyFromUnity(creditsOwned.ToString());
        #endif
    }

    public void playGame() {
        if (isAnimating) return;

        if (creditsOwned >= creditsNeeded) {
            RemoveCredits(creditsNeeded);
            didWin = Random.value < chanceToWin; // Decide win/loss up front
            playCraneAnimation();
        }
    }
    public void WinPrize() {
        rewardPanel.SetActive(true);
        Debug.Log("You win!");
        SpawnRandomPrize();
    }
    

    public void playCraneAnimation() {
        animator = clawMachine.GetComponent<Animator>();
        isAnimating = true;
        playButton.interactable = false;
        animator.Play("clawMachineAnimate", 0, 0f);
        StartCoroutine(WaitForCraneAnimation());
    }

    private IEnumerator WaitForCraneAnimation() {
        yield return null;
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength + 1f); // wait for animation to finish

        // Flash after animation win/loss
        Flashing flasher = clawMachine.GetComponent<Flashing>();

        if (didWin) {
            yield return StartCoroutine(flasher.FlashCoroutine(Color.green));
            WinPrize();
        } else {
            yield return StartCoroutine(flasher.FlashCoroutine(Color.red));
        }

        isAnimating = false;
        playButton.interactable = true;
    }

    public void SpawnRandomPrize() {

        int index = Random.Range(0, prizeObjects.Count);
        GameObject prizeToSpawn = prizeObjects[index];

        spawnedPrize = Instantiate(
            prizeToSpawn,
            spawnLocation.transform.position,
            Quaternion.identity,
            spawnContainer.transform
        );
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

    public void ReceiveBackgroundColor(string colorString) {
        if (ColorUtility.TryParseHtmlString(colorString, out Color parsedColor)) {
            if (mainCamera != null) {
                mainCamera.backgroundColor = parsedColor;
                Debug.Log("Background color set to: " + parsedColor);
            } else {
                Debug.LogWarning("Main camera is not assigned.");
            }
        } else {
            Debug.LogWarning("Invalid color string: " + colorString);
        }
    }
}

