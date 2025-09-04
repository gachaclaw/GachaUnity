using UnityEngine;
// Detect grabbed items and implements fake physics to make sure item motion is predictable
public class ClawGrabber : MonoBehaviour {
    public Capsule heldCapsule { get; private set; }
    public float freezeAboveY = 1.5f; // Set in Inspector
    private Capsule recentlyReleasedCapsule;
    private float releaseCooldownTime = 0.5f;
    private float releaseTimer = 0f;
    public LoadingScreen loadingScreen;
    private void Update() {
        if (releaseTimer > 0f) {
            releaseTimer -= Time.deltaTime;
            if (releaseTimer <= 0f) {
                recentlyReleasedCapsule = null;
            }
        }
    }
    private void OnTriggerStay(Collider other) {
        // Only proceed if the game is marked as ready
        if (!loadingScreen.gameIsReady) return;

        if (heldCapsule == null) {
            Capsule capsule = other.GetComponent<Capsule>();
            // Assigns capsule to claw if capsule reaches certain y-value
            if (capsule != null && capsule != recentlyReleasedCapsule && capsule.transform.position.y >= freezeAboveY) {
                heldCapsule = capsule;
                capsule.FreezeToCrane();
                Debug.Log($"[ClawGrabber] Grabbed capsule: {capsule.name} at Y={capsule.transform.position.y:F2}");
            }
        }
    }
    // Triggers before ClawOpen animation near drop zone
    public void ReleaseCapsule() {
        if (heldCapsule != null) {
            Debug.Log($"[ClawGrabber] Released capsule: {heldCapsule.name}");
            recentlyReleasedCapsule = heldCapsule;
            releaseTimer = releaseCooldownTime;

            heldCapsule.UnfreezeFromCrane();
            heldCapsule = null;
        }
    }
}