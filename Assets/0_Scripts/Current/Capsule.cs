using UnityEngine;
// Capsule object specific script
public class Capsule : MonoBehaviour {
    private Transform craneTransform;
    private Rigidbody rb;
    //private bool isHeld = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogWarning("No Rigidbody found on Capsule.");
        }

        GameObject craneObj = GameObject.FindWithTag("Crane");
        if (craneObj != null) {
            craneTransform = craneObj.transform;
        } else {
            Debug.LogWarning("No GameObject with tag 'Crane' found in scene.");
        }
    }

    public void FreezeToCrane() {
        rb.isKinematic = true;
        rb.freezeRotation = true;
        transform.SetParent(craneTransform);
        //isHeld = true;
    }

    public void UnfreezeFromCrane() {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.freezeRotation = false;
        //isHeld = false;
    }
}