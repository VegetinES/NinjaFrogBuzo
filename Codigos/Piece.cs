using UnityEngine;
using TMPro;

public class Piece : MonoBehaviour
{
    public int x;
    public int y;
    public bool hasBomb = false;
    private bool isRevealed = false;
    private bool isFlagged = false;
    
    private TextMeshProUGUI bombText;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        Transform canvasTransform = transform.Find("Canvas");
        if (canvasTransform != null)
        {
            Transform textTransform = canvasTransform.Find("Text (TMP)");
            if (textTransform == null)
                textTransform = canvasTransform.Find("Text");
            
            if (textTransform != null)
            {
                bombText = textTransform.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    void OnMouseDown()
    {
        // No hacer nada si el juego terminó
        if (Generator.gen.IsGameOver())
            return;
        
        // Click derecho para poner/quitar bandera
        if (Input.GetMouseButtonDown(1))
        {
            if (!isRevealed)
            {
                isFlagged = !isFlagged;
                if (bombText != null)
                {
                    bombText.text = isFlagged ? "🚩" : "";
                    bombText.fontSize = isFlagged ? 24 : 36;
                }
            }
            return;
        }
        
        // Click izquierdo para revelar
        if (isRevealed || isFlagged)
            return;

        isRevealed = true;

        if (hasBomb)
        {
            spriteRenderer.color = Color.red;
            if (bombText != null)
            {
                bombText.text = "💣";
                bombText.fontSize = 24;
            }
            Generator.gen.GameOver();
        }
        else
        {
            int bombCount = Generator.gen.CountBombsAround(x, y);
            
            if (bombText != null)
            {
                bombText.text = bombCount > 0 ? bombCount.ToString() : "";
                bombText.color = GetColorForNumber(bombCount);
            }
            
            spriteRenderer.color = Color.gray;
            
            // Auto-revelar casillas vacías 
            if (bombCount == 0)
            {
                Generator.gen.RevealAdjacentPieces(x, y);
            }
            
            // Verificar victoria después de cada jugada
            Generator.gen.CheckVictory();
        }
    }

    public void Reveal()
    {
        if (isRevealed || isFlagged || hasBomb)
            return;
            
        isRevealed = true;
        
        int bombCount = Generator.gen.CountBombsAround(x, y);
        
        if (bombText != null)
        {
            bombText.text = bombCount > 0 ? bombCount.ToString() : "";
            bombText.color = GetColorForNumber(bombCount);
        }
        
        spriteRenderer.color = Color.gray;
        
        if (bombCount == 0)
        {
            Generator.gen.RevealAdjacentPieces(x, y);
        }
    }

    public void ShowBomb()
    {
        spriteRenderer.color = Color.red;
        if (bombText != null)
        {
            bombText.text = "💣";
            bombText.fontSize = 24;
        }
    }

    public bool IsRevealed()
    {
        return isRevealed;
    }

    Color GetColorForNumber(int number)
    {
        switch (number)
        {
            case 1: return new Color(0f, 0f, 1f); // Azul
            case 2: return new Color(0f, 0.5f, 0f); // Verde oscuro
            case 3: return new Color(1f, 0f, 0f); // Rojo
            case 4: return new Color(0f, 0f, 0.5f); // Azul oscuro
            case 5: return new Color(0.5f, 0f, 0f); // Marrón
            case 6: return new Color(0f, 0.8f, 0.8f); // Cyan
            case 7: return Color.black; // Negro
            case 8: return new Color(0.3f, 0.3f, 0.3f); // Gris oscuro
            default: return Color.white;
        }
    }
}
