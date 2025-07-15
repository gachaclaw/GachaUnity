using UnityEngine;
using System.Collections;

public class Flashing : MonoBehaviour {
    [Header("Flash Settings")]
    public Color flashColor = Color.red;
    public int flashCount = 3;
    public float flashDuration = 0.2f;

    private Renderer rend;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Awake() {
        rend = GetComponent<Renderer>();
        if (rend != null) {
            originalColor = rend.material.color;
        } else {
            Debug.LogWarning("No Renderer found on this GameObject.");
        }
    }

    public void Flash(Color color) {
        if (rend == null) return;

        if (flashCoroutine != null) {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashCoroutine(color));
    }

    public IEnumerator FlashCoroutine(Color color) {
        if (rend == null) yield break;

        for (int i = 0; i < flashCount; i++) {
            rend.material.color = color;
            yield return new WaitForSeconds(flashDuration);
            rend.material.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        rend.material.color = originalColor;
    }
}
