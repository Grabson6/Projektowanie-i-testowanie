using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap wallTilemap;
    public TileBase leftWallTile;
    public TileBase rightWallTile;

    public Tilemap backgroundTilemap;
    public TileBase backgroundTile;

    public GameObject platformPrefab;
    public Transform player;

    public int levelWidth = 8;
    public float minY = 2.5f;
    public float maxY = 3.5f;
    public float minWidth = 2f;
    public float maxWidth = 4f;

    public float maxHorizontalJump = 5f;
    public float minHorizontalShift = 1.5f;

    public int initialTilesDown = 20;

    private float lastSpawnY;
    private float lastSpawnX;

    void Start()
    {
        lastSpawnY = player.position.y;
        lastSpawnX = 0f;

        float currentY = lastSpawnY;
        for (int i = 0; i < initialTilesDown; i++)
        {
            currentY -= 2f;
            PaintLayer(currentY);
        }

        for (int i = 0; i < 10; i++)
        {
            SpawnFloor();
        }
    }

    void Update()
    {
        if (player.position.y > lastSpawnY - 15f)
        {
            SpawnFloor();
        }
    }

    void SpawnFloor()
    {
        lastSpawnY += Random.Range(minY, maxY);
        PaintLayer(lastSpawnY);
        SpawnPlatform();
    }

    void PaintLayer(float yPos)
    {
        Vector3Int leftWallPos = wallTilemap.WorldToCell(new Vector3(-levelWidth, yPos, 0));
        Vector3Int rightWallPos = wallTilemap.WorldToCell(new Vector3(levelWidth - 1, yPos, 0));

        for (int y = -5; y <= 5; y++)
        {
            Vector3Int posL = new Vector3Int(leftWallPos.x, leftWallPos.y + y, 0);
            Vector3Int posR = new Vector3Int(rightWallPos.x, rightWallPos.y + y, 0);

            if (!wallTilemap.HasTile(posL)) wallTilemap.SetTile(posL, leftWallTile);
            if (!wallTilemap.HasTile(posR)) wallTilemap.SetTile(posR, rightWallTile);

            for (int x = -40; x < 40; x++)
            {
                Vector3Int backgroundPos = backgroundTilemap.WorldToCell(new Vector3(x, yPos + y, 0));
                if (!backgroundTilemap.HasTile(backgroundPos))
                    backgroundTilemap.SetTile(backgroundPos, backgroundTile);
            }
        }
    }

    void SpawnPlatform()
    {
        float width = Random.Range(minWidth, maxWidth);
        float wallBound = levelWidth - 2f - (width / 2f);

        float shift = Random.Range(minHorizontalShift, maxHorizontalJump);
        int direction = Random.Range(0, 2) == 0 ? -1 : 1;

        if (lastSpawnX < -wallBound + 2f) direction = 1;
        else if (lastSpawnX > wallBound - 2f) direction = -1;

        float newX = lastSpawnX + (shift * direction);
        newX = Mathf.Clamp(newX, -wallBound, wallBound);

        lastSpawnX = newX;

        Vector3 spawnPos = new Vector3(newX, lastSpawnY, -0.1f);

        GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);

        SpriteRenderer sr = newPlatform.GetComponent<SpriteRenderer>();
        if (sr != null) sr.size = new Vector2(width, sr.size.y);

        BoxCollider2D col = newPlatform.GetComponent<BoxCollider2D>();
        if (col != null) col.size = new Vector2(width, col.size.y);
    }
}