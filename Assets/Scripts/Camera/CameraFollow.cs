using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Asigna el Transform del jugador en el Inspector
    public Tilemap wallTilemap; // Asigna el Tilemap "wall" en el Inspector
    public float smoothTime = 0.3f; // Tiempo de suavizado para el movimiento de la cámara
    private Vector3 velocity = Vector3.zero;

    private BoundsInt tilemapBounds;
    private float cameraHalfWidth;
    private float cameraHalfHeight;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned! Please assign it in the inspector.");
        }

        if (wallTilemap == null)
        {
            Debug.LogError("Wall Tilemap is not assigned! Please assign the 'wall' Tilemap in the inspector.");
        }
        else
        {
            tilemapBounds = wallTilemap.cellBounds;
        }

        // Verifica si Camera.main no es nulo antes de usarlo
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera is not found! Please ensure there is a camera with the 'MainCamera' tag.");
            return;
        }

        // Calcula la mitad del ancho y alto de la cámara en unidades del mundo
        cameraHalfHeight = Camera.main.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        if (player == null || wallTilemap == null) return;

        Vector3 targetPosition = player.position;
        Vector3 clampedPosition = targetPosition;

        // Calcula los límites de la cámara basados en el Tilemap "wall"
        float minX = tilemapBounds.min.x + cameraHalfWidth;
        float maxX = tilemapBounds.max.x - cameraHalfWidth;
        float minY = tilemapBounds.min.y + cameraHalfHeight;
        float maxY = tilemapBounds.max.y - cameraHalfHeight;

        // Limita la posición de la cámara dentro de los límites del Tilemap
        clampedPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        clampedPosition.z = transform.position.z; // Mantén la profundidad de la cámara

        // Movimiento suave de la cámara
        transform.position = Vector3.SmoothDamp(transform.position, clampedPosition, ref velocity, smoothTime);
    }
}