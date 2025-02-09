using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGraphic : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Sprite[] sprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetColor(int type)
    {
        if(type == 0)
        {
            spriteRenderer.color = new Color(0, 0, 0,0);
        }
        else
        {
            int index = type - 1;

            spriteRenderer.color = Color.white;
            spriteRenderer.sprite = sprites[index];
        }
    }


}
