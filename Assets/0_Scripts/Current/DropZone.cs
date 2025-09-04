using UnityEngine;
// Detects prizes that fall in dropzone, log it, then delete the prize object
public class DropZone : MonoBehaviour {
    public ReactUnityBridge bridge;

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<SphereCollider>() != null) {
            bridge.prizesWon += 1;
            bridge.UpdatePrizesWon();
            Destroy(other.gameObject);
        }
    }
}