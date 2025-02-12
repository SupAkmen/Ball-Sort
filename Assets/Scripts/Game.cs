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

        //Debug.Log("Load level- gameGraphics");

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

    private Stack<List<SwitchBallCommand>> moveHistory = new Stack<List<SwitchBallCommand>>();

    public void SwitchBall(Bottle bottle1, Bottle bottle2)
    {
        List<Ball> bottle1Ball = bottle1.balls;
        List<Ball> bottle2Ball = bottle2.balls;

        int maxCapacity = GameLevelReader.instance.GetBallPerBottle(); // 👈 Số bóng tối đa trong bình

        if (bottle1Ball.Count == 0 || bottle2Ball.Count == maxCapacity)
            return;

        int index = bottle1Ball.Count - 1;
        Ball b = bottle1Ball[index];
        var type = b.type;

        if (bottle2Ball.Count > 0 && bottle2Ball[bottle2Ball.Count - 1].type != type)
            return;

        int maxMovableBalls = maxCapacity - bottle2Ball.Count; // 👈 Chỉ lấy số bóng có thể di chuyển
        if (maxMovableBalls == 0) return; // 👈 Không có chỗ để di chuyển, dừng ngay

        List<SwitchBallCommand> moves = new List<SwitchBallCommand>();
        int targetIndex = bottle2Ball.Count;
        int movedBalls = 0;

        for (int i = index; i >= 0 && movedBalls < maxMovableBalls; i--) // 👈 Không di chuyển quá giới hạn
        {
            Ball ball = bottle1Ball[i];
            if (ball.type == type)
            {
                moves.Add(new SwitchBallCommand
                {
                    type = type,
                    fromBottleIndex = bottles.IndexOf(bottle1),
                    toBottleIndex = bottles.IndexOf(bottle2),
                    fromBallIndex = i,
                    toBallIndex = targetIndex++
                });

                movedBalls++;
            }
            else
            {
                break;
            }
        }

        if (moves.Count == 0) return; // Nếu không có bóng hợp lệ để di chuyển, dừng ngay

        // Thực hiện di chuyển bóng*
        for (int i = 0; i < moves.Count; i++)
        {
            bottle1Ball.RemoveAt(bottle1Ball.Count - 1);
            bottle2Ball.Add(new Ball { type = type });
        }

        moveHistory.Push(moves); //  Lưu thao tác vào Stack để hỗ trợ Undo
        gameGraphic.RefreshBottleGraphics(bottles);
    }




    public void UndoMove()
    {
        if (moveHistory.Count == 0)
        {
            Debug.Log("Không có thao tác nào để hoàn tác!");
            return;
        }

        List<SwitchBallCommand> lastMoves = moveHistory.Pop();

        gameGraphic.StartCoroutine(gameGraphic.UndoMoveAnimation(lastMoves));

        //for (int i = lastMoves.Count - 1; i >= 0; i--)
        //{
        //    var move = lastMoves[i];

        //    Bottle fromBottle = bottles[move.toBottleIndex];
        //    Bottle toBottle = bottles[move.fromBottleIndex];

        //    if (fromBottle.balls.Count > 0)
        //    {
        //        Ball ball = fromBottle.balls[fromBottle.balls.Count - 1];

        //        if (ball.type == move.type)
        //        {
        //            fromBottle.balls.RemoveAt(fromBottle.balls.Count - 1);
        //            toBottle.balls.Add(ball);
        //        }
        //    }
        //}

        //gameGraphic.RefreshBottleGraphics(bottles);


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

        if (bottle2Ball.Count == GameLevelReader.instance.GetBallPerBottle())
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
