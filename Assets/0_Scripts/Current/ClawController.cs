using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
// Controls the claw movement, handles automatic movement sequences, fake physics, claw animations, restricted movement areas
public class ClawController : MonoBehaviour {
    [Header("Animator & State")]
    public Animator animator;
    //private bool isClawClosed = true;
    private bool isPerformingSequence = false;

    [Header("Movement Settings")]
    public float dragMovespeed = 5f;

    [Header("Tilt Settings")]
    public float tiltAmount = 10f;
    public float tiltSmoothSpeed = 5f;
    public float bumpTiltAmount = 5f;
    private Quaternion originalRotation;

    [Header("Crane Sequence Settings")]
    public float craneMoveDistance = 2f;
    public float craneMoveSpeed = 2f;
    public float delayBetweenActions = 0.5f;

    [Header("Ascent Settings")]
    public float ascentTargetHeight = 3.5f;

    [Header("Descent Settings")]
    public float minimumDescent = 0.5f;
    public float descentRayOffsetY = 0.5f;
    public float descentClearanceOffset = 0.1f;
    public LayerMask descentObstructionMask;


    [Header("Drop Zone Settings")]
    public Transform dropZoneTarget;        // Assign this in the inspector
    public Vector3 clawStartPosition = new Vector3(0f, 3.5f, 0f);  // Claw reset point

    [Header("Movement Settings")]
    public bool restrictToOneAxisMovement = true;

    [Header("DPAD Buttons")]
    public Button leftButton;
    public Button rightButton;
    public Button upButton;
    public Button downButton;
    public Button grabButton;

    public bool enableWASDMovement = true;

    public MeshCollider movementArea;

    private Capsule heldCapsule;

    private ClawGrabber clawGrabber;

    public ReactUnityBridge bridge;

    void Start() {
        clawGrabber = GetComponentInChildren<ClawGrabber>();
        originalRotation = transform.rotation;
        leftButton.onClick.AddListener(() => MoveClawOnce(Vector3.left));
        rightButton.onClick.AddListener(() => MoveClawOnce(Vector3.right));
        upButton.onClick.AddListener(() => MoveClawOnce(Vector3.forward));
        downButton.onClick.AddListener(() => MoveClawOnce(Vector3.back));
        grabButton.onClick.AddListener(() => TriggerGrab());


    }

    void Update() {
        if (!isPerformingSequence && enableWASDMovement) {
            HandleMovement();
        } else if (!isPerformingSequence) {
            ResetTiltWhenIdle();
        }
        /*
        if (!isPerformingSequence && Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(PerformClawSequence());
        }
        */

        /*
        if (!isPerformingSequence && Input.GetKeyDown(KeyCode.G)) {
            if (isClawClosed) {
                animator.Play("ClawOpen");
                isClawClosed = false;
            }
        }
        
        if (!isPerformingSequence && Input.GetKeyDown(KeyCode.J)) {
            StartCoroutine(MoveCrane(Vector3.down));
        }

        if (!isPerformingSequence && Input.GetKeyDown(KeyCode.K)) {
            StartCoroutine(MoveCrane(Vector3.up));
        }
        */
    }

    void ResetTiltWhenIdle() {
        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * tiltSmoothSpeed);
    }

    bool IsWithinMovementArea(Vector3 pos) {
        if (movementArea == null) return false;
        return IsPointInsideMesh(movementArea, pos);
    }
    bool IsPointInsideMesh(MeshCollider meshCol, Vector3 point) {
        Vector3 dir = Vector3.down; // Raycast points down into the mesh
        Ray ray = new Ray(point, dir);
        int hitCount = 0;
        RaycastHit hit;

        float maxTestDistance = 100f;
        if (meshCol.Raycast(ray, out hit, maxTestDistance)) {
            hitCount++;
            float offset = 0.01f;
            Vector3 newOrigin = hit.point + dir * offset;
            while (meshCol.Raycast(new Ray(newOrigin, dir), out hit, maxTestDistance)) {
                hitCount++;
                newOrigin = hit.point + dir * offset;
            }
        }

        return (hitCount % 2) == 1;
    }
    void HandleMovement() {
        Debug.Log("HandleMovement started");

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Disables diagonal movement
        if (restrictToOneAxisMovement && Mathf.Abs(horizontal) > 0f && Mathf.Abs(vertical) > 0f) {
            vertical = 0f;
        }

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.sqrMagnitude > 0f) {
            inputDirection.Normalize();
            Vector3 proposedMovement = inputDirection * dragMovespeed * Time.deltaTime;
            Vector3 targetPos = transform.position + proposedMovement;
            // Checks if inside allowed bounds
            if (IsWithinMovementArea(targetPos)) {
                transform.position = targetPos;
            }
        }

        // TILT
        Quaternion targetRotation = originalRotation;
        if (inputDirection != Vector3.zero && movementArea != null) {
            Vector3 pos = transform.position;
            Bounds bounds = movementArea.bounds;

            float xEdge = Mathf.Min(Mathf.Abs(pos.x - bounds.min.x), Mathf.Abs(pos.x - bounds.max.x));
            float zEdge = Mathf.Min(Mathf.Abs(pos.z - bounds.min.z), Mathf.Abs(pos.z - bounds.max.z));

            float edgeBuffer = 1f;
            float xFactor = Mathf.Clamp01(xEdge / edgeBuffer);
            float zFactor = Mathf.Clamp01(zEdge / edgeBuffer);

            float tiltX = inputDirection.z * tiltAmount * zFactor;
            float tiltZ = -inputDirection.x * tiltAmount * xFactor;

            if (xFactor <= 0.01f && zFactor <= 0.01f) {
                targetRotation = originalRotation;
            } else {
                targetRotation = originalRotation * Quaternion.Euler(tiltX, 0f, tiltZ);
            }
        }
        //Smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSmoothSpeed);
    }

    public void MoveClawOnce(Vector3 direction) {
        Debug.Log("MoveClawOnce started");
        if (isPerformingSequence) return;

        Vector3 inputDirection = direction.normalized;
        Vector3 proposedMovement = inputDirection * dragMovespeed * Time.deltaTime;
        Vector3 targetPos = transform.position + proposedMovement;
        // Checks if inside allowed bounds
        if (!IsWithinMovementArea(targetPos)) {
            Debug.Log("IsWithinMovementArea debug");
            return;
        }

        transform.position = targetPos;

        // TILT
        Quaternion targetRotation = originalRotation;
        Vector3 pos = transform.position;
        Bounds bounds = movementArea.bounds;

        float xEdge = Mathf.Min(Mathf.Abs(pos.x - bounds.min.x), Mathf.Abs(pos.x - bounds.max.x));
        float zEdge = Mathf.Min(Mathf.Abs(pos.z - bounds.min.z), Mathf.Abs(pos.z - bounds.max.z));

        float edgeBuffer = 1f;
        float xFactor = Mathf.Clamp01(xEdge / edgeBuffer);
        float zFactor = Mathf.Clamp01(zEdge / edgeBuffer);

        float tiltX = inputDirection.z * tiltAmount * zFactor;
        float tiltZ = -inputDirection.x * tiltAmount * xFactor;

        if (xFactor <= 0.01f && zFactor <= 0.01f) {
            targetRotation = originalRotation;
        } else {
            targetRotation = originalRotation * Quaternion.Euler(tiltX, 0f, tiltZ);
        }
        //Smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSmoothSpeed);
    }


    IEnumerator PerformClawSequence() {
        isPerformingSequence = true;

        yield return ResetSwayRotation();

        yield return MoveCrane(Vector3.down);
        yield return new WaitForSeconds(delayBetweenActions);

        animator.Play("ClawClose");
        yield return new WaitForSeconds(2f);
        //isClawClosed = true;

        yield return MoveCrane(Vector3.up);
        yield return new WaitForSeconds(delayBetweenActions);

        // Move to drop zone (X then Z)
        if (dropZoneTarget != null) {
            Vector3 dropPos = dropZoneTarget.position;
            yield return MoveCraneToX(dropPos.x);
            yield return MoveCraneToZ(dropPos.z);
        }
        // 
        if (clawGrabber != null) {
            clawGrabber.ReleaseCapsule();
        }

        // 
        animator.Play("ClawOpen");
        yield return new WaitForSeconds(2f);

        // Return to start position (X then Z)
        yield return MoveCraneToX(clawStartPosition.x);
        yield return MoveCraneToZ(clawStartPosition.z);

        isPerformingSequence = false;
    }

    IEnumerator MoveCrane(Vector3 direction) {
        if (direction == Vector3.down) {
            // Determine descent limit
            Vector3 origin = transform.position + new Vector3(0, descentRayOffsetY, 0);
            RaycastHit hit;
            Debug.DrawRay(origin, Vector3.down * craneMoveDistance, Color.red, 2f);
            float descentLimit = minimumDescent;
            bool hitObject = Physics.Raycast(origin, Vector3.down, out hit, craneMoveDistance, descentObstructionMask);

            if (hitObject) {
                float hitY = hit.point.y;
                descentLimit = Mathf.Max(hitY + descentClearanceOffset, minimumDescent);

            }

            Vector3 start = transform.position;
            Vector3 end = new Vector3(start.x, descentLimit, start.z);

            bool bumped = false;
            float elapsed = 0f;

            while (elapsed < 1f && transform.position.y > descentLimit + 0.01f) {
                elapsed += Time.deltaTime * craneMoveSpeed;
                transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed));

                // Trigger bump tilt close to the obstacle
                if (!bumped && transform.position.y <= descentLimit + 0.15f) {
                    bumped = true;
                    Debug.Log("Bumped!");
                    Quaternion bumpTilt = Quaternion.Euler(bumpTiltAmount, 0f, 0f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation * bumpTilt, 0.5f);
                }

                yield return null;
            }
        } else {
            // Reset tilt immediately before ascending
            transform.rotation = originalRotation;

            // Custom upward movement to fixed height
            Vector3 start = transform.position;
            Vector3 end = new Vector3(start.x, ascentTargetHeight, start.z);

            float elapsed = 0f;
            while (elapsed < 1f && transform.position.y < ascentTargetHeight - 0.01f) {
                elapsed += Time.deltaTime * craneMoveSpeed;
                transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed));
                yield return null;
            }
        }
    }


    IEnumerator ResetSwayRotation() {
        float duration = 0.3f;
        float elapsed = 0f;

        Quaternion startRot = transform.rotation;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);
            yield return null;
        }
    }

    IEnumerator MoveCraneToX(float targetX) {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetX, start.y, start.z);
        float elapsed = 0f;

        while (elapsed < 1f && Mathf.Abs(transform.position.x - targetX) > 0.01f) {
            elapsed += Time.deltaTime * craneMoveSpeed;
            transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed));
            yield return null;
        }
    }

    IEnumerator MoveCraneToZ(float targetZ) {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(start.x, start.y, targetZ);
        float elapsed = 0f;

        while (elapsed < 1f && Mathf.Abs(transform.position.z - targetZ) > 0.01f) {
            elapsed += Time.deltaTime * craneMoveSpeed;
            transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed));
            yield return null;
        }
    }


    public void TriggerGrab() {
        if (!isPerformingSequence && (bridge.creditsOwned >= bridge.creditsNeeded)) {
            bridge.RemoveCredits(bridge.creditsNeeded);
            StartCoroutine(PerformClawSequence());
        }
    }
}