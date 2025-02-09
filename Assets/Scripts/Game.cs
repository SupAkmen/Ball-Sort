using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameGraphic gameGraphic;
    public List<Bottle> bottles;

    private void Start()
    {

        GameManager.onGameStateChanged += GameStateChangedCallBack;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallBack;
    }
    private void GameStateChangedCallBack(GameState gameState)
    {
        switch (gameState)
        {
            
            case GameState.Game:
                
                break;
             
        }
    }


    public void LoadLevel(List<int[]> listArray)
    {
        bottles = new List<Bottle>();

        foreach (int[] arr in listArray)
        {
            Bottle b = new Bottle();
            // quan trong
            b.balls = new List<Ball>(); // Khởi tạo balls ở đây

            for (int i = 0; i < arr.Length; i++)
            {
                int element = arr[i];
                if (element == 0)
                    continue;
                b.balls.Add(new Ball
                {
                    type = element
                });
            }

            bottles.Add(b);
        }

        //Debug.Log("Load level- game");

            gameGraphic.CreateBottleGraphic(bottles);
        
    }


    //public void PrintBottles()
    //{
    //    Debug.Log("Bottles======");

    //    StringBuilder sb = new StringBuilder();

    //    for (int i = 0; i < bottles.Count; i++)
    //    {
    //        Bottle b= bottles[i];
    //        sb.Append("Bottle " + (i + 1) + ":");
    //        foreach(Ball ball in b.balls)
    //        {
    //            sb.Append(" " + ball.type);
    //            sb.Append(",");
    //        }

    //        Debug.Log(sb.ToString());
    //        sb.Clear();
    //    }

    //    bool isWin = CheckWinCondition();

    //    Debug.Log(isWin);

    //}

    public void SwitchBall(Bottle bottle1, Bottle bottle2)
    {
        List<Ball> bottle1Ball = bottle1.balls;
        List<Ball> bottle2Ball = bottle2.balls;

        if (bottle1Ball.Count == 0)
            return;

        if (bottle2Ball.Count == 4)
            return;

        // bong cao nhat
        int index = bottle1Ball.Count - 1;
        Ball b = bottle1Ball[index];

        var type = b.type;

        if(bottle2Ball.Count > 0 && bottle2Ball[bottle2Ball.Count - 1].type != type)
        {
            return;
        }
        for(int i = index; i >=0; i--)
        {
            Ball ball = bottle1Ball[i];
            if(ball.type == type)
            {
                bottle1Ball.RemoveAt(i);
                bottle2Ball.Add(b);

                if(bottle2Ball.Count == 4)
                {  break; }

            }
            else
            {
                break;
            }
        }
    }

    public void SwitchBall(int bottleIndex1, int bottleIndex2)
    {
        Bottle b1 = bottles[bottleIndex1];
        Bottle b2 = bottles[bottleIndex2];

        SwitchBall(b1, b2);

        gameGraphic.RefreshBottleGraphics(bottles);
    }    

    public bool CheckWinCondition()
    {
        bool winFlag = true;

        foreach(Bottle bottle in bottles)
        {
            if(bottle.balls.Count == 0)
            {
                continue;
            }

            if(bottle.balls.Count < 4)
            {
                winFlag = false;
                break;
            }


            bool sameTypeFlag = true;
            int type = bottle.balls[0].type;

            foreach(Ball ball in bottle.balls)
            {
                if (ball.type != type)
                {
                    sameTypeFlag = false;
                    break;
                }
            }

            if(!sameTypeFlag)
            {
                winFlag = false;
                break;
            }
        }

        return winFlag;
    }

    public List<SwitchBallCommand> CheckSwitchBall(int bottleIndex1,int bottleIndex2)
    {
        List<SwitchBallCommand>commands = new List<SwitchBallCommand>();

        Bottle bottle1 = bottles[bottleIndex1];
        Bottle bottle2 = bottles[bottleIndex2];

        List<Ball> bottle1Ball = bottle1.balls;
        List<Ball> bottle2Ball = bottle2.balls;

        if (bottle1Ball.Count == 0)
            return commands;

        if (bottle2Ball.Count == 4)
            return commands;

        int index = bottle1Ball.Count - 1;
        Ball b = bottle1Ball[index];

        var type = b.type;

        if (bottle2Ball.Count > 0 && bottle2Ball[bottle2Ball.Count - 1].type != type)
        {
            return commands;
        }

        int targetIndex = bottle2Ball.Count;

        for (int i = index; i >= 0; i--)
        {
            Ball ball = bottle1Ball[i];
            if (ball.type == type)
            {
                int fromBallIndex = i;
                int toBallIndex = targetIndex;
                int fromBottleIndex = bottleIndex1;
                int toBottleIndex = bottleIndex2;

                commands.Add(new SwitchBallCommand
                {
                    type = type,
                    fromBallIndex = fromBallIndex,
                    toBallIndex = toBallIndex,
                    fromBottleIndex = fromBottleIndex,
                    toBottleIndex = toBottleIndex,
                }); ; 

                targetIndex++;

                if (targetIndex == 4)
                { break; }

            }
            else
            {
                break;
            }
        }

        return commands;
    }
    public class SwitchBallCommand
    {
        public int type;

        public int fromBottleIndex;
        public int fromBallIndex;

        public int toBottleIndex;
        public int toBallIndex;
    }
   public class Bottle
    {
        public List<Ball> balls;
    }

    public class Ball
    {
        public int type;
    }


}
