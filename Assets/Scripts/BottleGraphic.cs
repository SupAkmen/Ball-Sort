using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BottleGraphic : MonoBehaviour
{
    public GameGraphic gameGraphic;
    public int index;
    public BallGraphic[] ballGraphics;
    public Transform bottleUpTransform;

    private void OnMouseUpAsButton()
    {
        gameGraphic.OnClickBottle(index);
    }

    public void SetGraphic(int[] ballTypes)
    {
        for(int i = 0; i < ballGraphics.Length; i++)
        {
            if(i >= ballTypes.Length)
            {
                SetGraphicNone(i);
            }
            else
            {
                SetGraphic(i, ballTypes[i]);
            }
        }
    }   

    public void SetGraphic(int index, int type)
    {
        ballGraphics[index].SetColor(type);
    }

    public void SetGraphicNone(int index)
    {
        ballGraphics[index].SetColor(0);
    }


    public Vector3 GetBallPosition(int index)
    {
        return ballGraphics[index].transform.position;
    }

    public Vector3 GetBottleUpPosition()
    {
        return bottleUpTransform.position;
    }
}
