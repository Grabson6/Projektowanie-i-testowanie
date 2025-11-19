using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [Header("Elementy do budowy")]
    public Tilemap wallTilemap;
    public TileBase leftWallTile;
    public TileBase rightWallTile;

    public Tilemap backgroundTilemap;
    public TileBase backgroundTile;

    public GameObject platformPrefab;
    public Transform player;

    [Header("Ustawienia Generowania")]
    public int levelWidth = 8;
    public float minY = 2.5f;
    public float maxY = 4.5f;
    public float minWidth = 2f;
    public float maxWidth = 5f;

    [Header("Ustawienia Startowe")]
    public int initialTilesDown = 10;   // Ile segmentow wygenerowac ponizej startu
    public float initialYSpacing = 2f;  // Odstep pionowy miedzy segmentami na start

    private float lastSpawnY;

    // NOWA FUNKCJA: Maluje tylko sciany i tło na danej wysokosci, bez spawnowania platform
    void PaintVerticalSlice(float currentY)
    {
        Vector3Int leftWallPos = wallTilemap.WorldToCell(new Vector3(-levelWidth, currentY, 0));
        Vector3Int rightWallPos = wallTilemap.WorldToCell(new Vector3(levelWidth - 1, currentY, 0));

        for (int y = -5; y <= 5; y++)
        {
            Vector3Int posL = new Vector3Int(leftWallPos.x, leftWallPos.y + y, 0);
            Vector3Int posR = new Vector3Int(rightWallPos.x, rightWallPos.y + y, 0);

            // 1. STAWIANIE ŚCIAN BOCZNYCH
            if (!wallTilemap.HasTile(posL)) wallTilemap.SetTile(posL, leftWallTile);
            if (!wallTilemap.HasTile(posR)) wallTilemap.SetTile(posR, rightWallTile);

            // 2. GENEROWANIE TŁA W SZEROKIM ZAKRESIE
            for (int x = -40; x < 40; x++)
            {
                Vector3Int backgroundPos = backgroundTilemap.WorldToCell(new Vector3(x, currentY + y, 0));

                // Malujemy cegłę TYLKO w obszarze wieży
                if (x >= -levelWidth + 1 && x < levelWidth - 1)
                {
                    if (!backgroundTilemap.HasTile(backgroundPos))
                    {
                        backgroundTilemap.SetTile(backgroundPos, backgroundTile);
                    }
                }
                // Malujemy tło (cienia) POZA wieżą
                else
                {
                    if (!backgroundTilemap.HasTile(backgroundPos))
                    {
                        backgroundTilemap.SetTile(backgroundPos, backgroundTile);
                    }
                }
            }
        }
    }

    void Start()
    {
        lastSpawnY = player.position.y;

        // 1. GENEROWANIE W DÓŁ (NOWE)
        float currentY = lastSpawnY;
        for (int i = 0; i < initialTilesDown; i++)
        {
            // Przesuwamy w dół o stałą odległość
            currentY -= initialYSpacing;
            PaintVerticalSlice(currentY);
        }

        // 2. GENEROWANIE W GÓRĘ (jak było)
        for (int i = 0; i < 10; i++)
        {
            SpawnFloor(); // Ta funkcja generuje platformy i kontynuuje malowanie
        }
    }

    // Zmodyfikowana metoda - używa PaintVerticalSlice
    void SpawnFloor()
    {
        lastSpawnY += Random.Range(minY, maxY);

        PaintVerticalSlice(lastSpawnY); // Najpierw malujemy ściany i tło
        SpawnPlatform(); // Potem spawnuje platformę
    }

    void Update()
    {
        if (player.position.y > lastSpawnY - 15f)
        {
            SpawnFloor();
        }
    }

    void SpawnPlatform()
    {
        float width = Random.Range(minWidth, maxWidth);

        float safeZone = levelWidth - 2f - (width / 2f);
        float xPos = Random.Range(-safeZone, safeZone);

        Vector3 spawnPos = new Vector3(xPos, lastSpawnY, -0.1f);

        GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);

        SpriteRenderer sr = newPlatform.GetComponent<SpriteRenderer>();
        if (sr != null) sr.size = new Vector2(width, sr.size.y);

        BoxCollider2D col = newPlatform.GetComponent<BoxCollider2D>();
        if (col != null) col.size = new Vector2(width, col.size.y);
    }
}