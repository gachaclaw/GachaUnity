using UnityEngine;

public class PopAnimation : MonoBehaviour {
    public float popDuration = 0.3f;
    public float popScale = 1.5f;
    private Vector3 originalScale;
    private float timer;
    private bool popping;

    private void Awake() {
        originalScale = transform.localScale;
    }

    public void PlayPop() {
        popping = true;
        timer = 0f;
    }

    private void Update() {
        if (!popping) return;

        timer += Time.deltaTime;
        float progress = timer / popDuration;

        if (progress >= 1f) {
            progress = 1f;
            popping = false;
        }

        // Scale up and then back to original
        float scale = Mathf.Lerp(popScale, 1f, progress);
        transform.localScale = originalScale * scale;
    }
}