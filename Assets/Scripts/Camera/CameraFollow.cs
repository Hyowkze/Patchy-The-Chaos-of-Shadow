using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    
    [Header("Offset Settings")]
    [SerializeField] private Vector3 baseOffset = new Vector3(0f, 2f, -10f);
    [SerializeField] private float lookAheadDistance = 3f;
    [SerializeField] private float lookAheadSpeed = 2f;

    private Vector3 currentVelocity;
    private float currentLookAhead;
    private PlayerInputHandler inputHandler;

    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate look ahead based on player's input
        float targetLookAhead = inputHandler.MoveInput.x * lookAheadDistance;
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, Time.deltaTime * lookAheadSpeed);

        // Calculate desired position with dynamic offset
        Vector3 desiredPosition = target.position + baseOffset;
        desiredPosition.x += currentLookAhead;

        // Smooth follow
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / smoothSpeed
        );
    }
}
