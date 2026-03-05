using UnityEngine;

public class Generator : MonoBehaviour
{
    public static Generator gen;

    [Header("Configuración del Mapa")]
    public GameObject piecePrefab;
    public int width = 10;
    public int height = 10;
    public int bombsNumber = 15;

    private Piece[,] pieces;
    private bool gameOver = false;

    void Awake()
    {
        gen = this;
    }

    void Start()
    {
        GenerateMap();
        CenterCamera();
        GenerateBombs();
    }

    void GenerateMap()
    {
        pieces = new Piece[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject pieceObj = Instantiate(piecePrefab, new Vector3(x, y, 0), Quaternion.identity);
                pieceObj.transform.parent = transform;
                pieceObj.name = $"Piece ({x},{y})";
                
                Piece pieceScript = pieceObj.GetComponent<Piece>();
                pieceScript.x = x;
                pieceScript.y = y;
                pieces[x, y] = pieceScript;
            }
        }
    }

    void CenterCamera()
    {
        Camera.main.transform.position = new Vector3(width / 2f - 0.5f, height / 2f - 0.5f, -10);
        Camera.main.orthographicSize = Mathf.Max(width, height) / 2f + 1;
    }

    void GenerateBombs()
    {
        int bombsPlaced = 0;

        while (bombsPlaced < bombsNumber)
        {
            int randomX = Random.Range(0, width);
            int randomY = Random.Range(0, height);

            if (!pieces[randomX, randomY].hasBomb)
            {
                pieces[randomX, randomY].hasBomb = true;
                bombsPlaced++;
            }
        }
    }

    public int CountBombsAround(int x, int y)
    {
        int count = 0;

        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (offsetX == 0 && offsetY == 0)
                    continue;

                int checkX = x + offsetX;
                int checkY = y + offsetY;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    if (pieces[checkX, checkY].hasBomb)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    public void RevealAdjacentPieces(int x, int y)
    {
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (offsetX == 0 && offsetY == 0)
                    continue;

                int checkX = x + offsetX;
                int checkY = y + offsetY;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    pieces[checkX, checkY].Reveal();
                }
            }
        }
    }

    public void GameOver()
    {
        if (gameOver)
            return;
            
        gameOver = true;
        Debug.Log("💥 ¡GAME OVER! Perdiste");
        
        // Revelar todas las bombas
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y].hasBomb)
                {
                    pieces[x, y].ShowBomb();
                }
            }
        }
    }

    public void CheckVictory()
    {
        if (gameOver)
            return;
            
        int revealedCount = 0;
        int totalSafePieces = (width * height) - bombsNumber;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Piece piece = pieces[x, y];
                if (piece.IsRevealed() && !piece.hasBomb)
                {
                    revealedCount++;
                }
            }
        }
        
        if (revealedCount == totalSafePieces)
        {
            gameOver = true;
            Debug.Log("🎉 ¡VICTORIA! ¡Has ganado el juego!");
        }
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}
