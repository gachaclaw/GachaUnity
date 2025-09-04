using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Loading screen image stays on screen while game loads startup code
public class LoadingScreen : MonoBehaviour {
    [Header("UI Panel to show")]
    public GameObject panelToEnable;

    [Header("Duration panel stays active (in seconds)")]
    public float displayDuration = 3f;

    public bool gameIsReady = false;

    void Start() {
        if (panelToEnable != null) {
            StartCoroutine(ShowPanelTemporarily());
        } else {
            Debug.LogWarning("Panel reference not assigned.");
        }
    }

    private IEnumerator ShowPanelTemporarily() {
        panelToEnable.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        panelToEnable.SetActive(false);
        gameIsReady = true;
    }
}
