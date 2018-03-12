using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Texture2D _crosshairSprite;

    void Awake()
    {
        Cursor.SetCursor(_crosshairSprite, Vector2.one * (32 / 2), CursorMode.Auto);
    }
}