using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDebugger : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("===========================================");
        Debug.Log("=== SCENE DEBUGGER - GAMESCENE ===");
        Debug.Log("===========================================");
        
        // Buscar todos los elementos importantes
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        Debug.Log("Total de GameObjects en escena: " + allObjects.Length);
        
        // Buscar Grid
        GameObject grid = GameObject.Find("Grid");
        if (grid != null)
        {
            Debug.Log("✅ GRID encontrado");
            Debug.Log("   - Posición: " + grid.transform.position);
            Debug.Log("   - Activo: " + grid.activeInHierarchy);
            Debug.Log("   - Layer: " + LayerMask.LayerToName(grid.layer));
            
            // Buscar Tilemap
            Transform tilemap = grid.transform.Find("Tilemap");
            if (tilemap != null)
            {
                Debug.Log("✅ TILEMAP encontrado");
                Debug.Log("   - Posición: " + tilemap.position);
                Debug.Log("   - Activo: " + tilemap.gameObject.activeInHierarchy);
                
                var renderer = tilemap.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Debug.Log("   - Renderer activo: " + renderer.enabled);
                    Debug.Log("   - Bounds: " + renderer.bounds);
                }
            }
            else
            {
                Debug.LogWarning("⚠️ TILEMAP NO encontrado dentro de Grid!");
            }
        }
        else
        {
            Debug.LogError("❌ GRID NO ENCONTRADO!");
        }
        
        // Buscar todas las cámaras
        Camera[] cameras = FindObjectsOfType<Camera>();
        Debug.Log("Cámaras en escena: " + cameras.Length);
        foreach (Camera cam in cameras)
        {
            Debug.Log("   📷 Cámara: " + cam.name);
            Debug.Log("      - Posición: " + cam.transform.position);
            Debug.Log("      - Activa: " + cam.enabled);
            Debug.Log("      - Tag: " + cam.tag);
            Debug.Log("      - Projection: " + cam.orthographic);
            Debug.Log("      - Size: " + cam.orthographicSize);
        }
        
        Debug.Log("===========================================");
    }
}
