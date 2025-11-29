using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Cele")]
    public Transform player;        // Twój gracz

    [Header("Ustawienia Kamery")]
    public float smoothSpeed = 0.125f;  // Jak miękko kamera podąża
    public Vector3 offset;              // Odległość kamery od gracza
    public bool onlyUp = false;         // Zaznacz, żeby kamera nie spadała w dół

    [Header("Martwa Strefa (Dead Zone)")]
    public float deadZoneX = 2f;        // Szerokość martwej strefy (połowa, czyli 2 = 4 jednostki szerokości)
    public float deadZoneY = 1f;        // Wysokość martwej strefy (połowa, czyli 1 = 2 jednostki wysokości)

    private float highestY;             // Zapamiętuje najwyższy punkt (do trybu onlyUp)

    void Start()
    {
        // Ustaw początkowe najwyższe Y kamery
        highestY = transform.position.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = transform.position; // Zaczynamy od obecnej pozycji kamery

        // --- Obsługa Martwej Strefy w osi X (lewo/prawo) ---
        // Obliczamy różnicę między graczem a centrum kamery
        float deltaX = player.position.x - transform.position.x;

        // Jeśli gracz wyszedł poza lewą krawędź Dead Zone
        if (deltaX < -deadZoneX)
        {
            targetPosition.x = player.position.x + deadZoneX;
        }
        // Jeśli gracz wyszedł poza prawą krawędź Dead Zone
        else if (deltaX > deadZoneX)
        {
            targetPosition.x = player.position.x - deadZoneX;
        }

        // --- Obsługa Martwej Strefy w osi Y (góra/dół) ---
        float deltaY = player.position.y - transform.position.y;

        // Jeśli gracz wyszedł poza dolną krawędź Dead Zone
        if (deltaY < -deadZoneY)
        {
            targetPosition.y = player.position.y + deadZoneY;
        }
        // Jeśli gracz wyszedł poza górną krawędź Dead Zone
        else if (deltaY > deadZoneY)
        {
            targetPosition.y = player.position.y - deadZoneY;
        }

        // --- Tryb Icy Tower "Only Up" ---
        if (onlyUp)
        {
            // Zawsze upewniamy się, że kamera nie spada poniżej highestY
            if (targetPosition.y > highestY)
            {
                highestY = targetPosition.y;
            }
            targetPosition.y = highestY;
        }

        // --- Wygładzanie i aplikowanie pozycji ---
        // Zawsze zachowujemy Z kamery (żeby nie zniknęła)
        targetPosition.z = transform.position.z;

        // Wygładzamy ruch kamery do nowej docelowej pozycji
        transform.position = Vector3.Lerp(transform.position, targetPosition + offset, smoothSpeed);
    }
}