using UnityEngine;
using UnityEngine.EventSystems;
// Enables holding down of dpad button
public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Vector3 moveDirection;
    public ClawController clawController;
    private bool isHeld = false;

    void Update() {
        if (isHeld && clawController != null) {
            clawController.MoveClawOnce(moveDirection);
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        isHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isHeld = false;
    }
}
