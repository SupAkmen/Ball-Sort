using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameGraphic : MonoBehaviour
{
    public int selectedBottleIndex = -1;

    private Game game;

    public List<BottleGraphic> bottleGraphics;

    public BallGraphic prefabBallGraphics;

    public BottleGraphic prefabBottleGraphics;

    private BallGraphic previewBall;

    public Vector3 bottleStartPos;
    public Vector3 bottleDistance;

    public GameObject winEffect;
    private void OnEnable()
    {
        GameManager.onGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.onGameStateChanged -= OnGameStateChanged;
    }
    private void Start()
    {
        game = FindObjectOfType<Game>();
        selectedBottleIndex = -1;


        previewBall = Instantiate(prefabBallGraphics);
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.Game)
        {
            // Chuyển sang panel Game, hiển thị và tạo các bình
            ShowBottles();
            
        }
        else
        {
           
            HideBottles();
        }
    }

    public void CreateBottleGraphic(List<Game.Bottle> bottles)
    {
        foreach (Game.Bottle b in bottles)
        {
            BottleGraphic bg = Instantiate(prefabBottleGraphics);


            bottleGraphics.Add(bg);

            List<int> ballTypes = new List<int>();

            foreach (var ball in b.balls)
            {
                ballTypes.Add(ball.type);
            }

            bg.SetGraphic(ballTypes.ToArray());

        }

        Vector3 pos = bottleStartPos;

        for(int i=0; i<bottleGraphics.Count; i++)
        {
            bottleGraphics[i].transform.position = pos;

            pos.x += bottleDistance.x;

            bottleGraphics[i].index = i;

            //Debug.Log("Spawn");
        }
    }

    private void ShowBottles()
    {
        foreach (var bottle in bottleGraphics)
        {
            bottle.gameObject.SetActive(true); // Hiển thị các bình
        }
    }

    private void HideBottles()
    {
        foreach (var bottle in bottleGraphics)
        {
            bottle.gameObject.SetActive(false); // Ẩn các bình
        }
    }

    public void ClearBottleGraphics()
    {
        foreach (var bottleGraphic in bottleGraphics)
        {
            Destroy(bottleGraphic.gameObject); // Xóa đối tượng bình khỏi scene
        }
        bottleGraphics.Clear(); // Xóa danh sách bình chứa
    }

    public void RefreshBottleGraphics(List<Game.Bottle> bottles)
   {
        for(int i = 0; i < bottles.Count; i++)
        {
            Game.Bottle gb = bottles[i];
            BottleGraphic bottleGraphic = bottleGraphics[i];

            List<int> ballTypes = new List<int>();

            foreach(var ball in gb.balls)
            {
                ballTypes.Add(ball.type);
            }

            bottleGraphic.SetGraphic(ballTypes.ToArray());
        }
   }

    public void OnClickBottle(int bottleIndex)
    {
       // Debug.Log(bottleIndex);

        if (isSwitchingBall)
            return;
        // trang thai mac dinh : -1
        // khi co ball : bottleIndex

        if(selectedBottleIndex == -1)
        {
            if (game.bottles[bottleIndex].balls.Count != 0)
            {
                selectedBottleIndex = bottleIndex;
                StartCoroutine(MoveBallUp(bottleIndex));
            }
        }
        else
        {

            if(selectedBottleIndex == bottleIndex)
            {
                StartCoroutine(MoveBallDown(bottleIndex));
                selectedBottleIndex = -1;
            }
            else
            {
               
                StartCoroutine(SwitchBallCoroutine(selectedBottleIndex, bottleIndex));

                if (game.CheckWinCondition())
                {
                    Debug.Log("Win 1");
                }


            }
        }    
    }

    private IEnumerator MoveBallUp(int bottleIndex)
    {
        isSwitchingBall = true;
        Vector3 upPosition = bottleGraphics[bottleIndex].GetBottleUpPosition();

        List<Game.Ball> ballList = game.bottles[bottleIndex].balls;

        // lay bong cao
        Game.Ball b = ballList[ballList.Count - 1];

        Vector3 ballPosition = bottleGraphics[bottleIndex].GetBallPosition(ballList.Count - 1);

        bottleGraphics[bottleIndex].SetGraphicNone(ballList.Count - 1);

        previewBall.SetColor((b.type));
        previewBall.transform.position = ballPosition;

        previewBall.gameObject.SetActive(true);

        while(Vector2.Distance(previewBall.transform.position, upPosition)  > 0.001f)
        {
            previewBall.transform.position = Vector3.MoveTowards(previewBall.transform.position,upPosition,10 * Time.deltaTime);
            yield return null;
        }
        isSwitchingBall=false;
    }

    private IEnumerator MoveBallDown(int bottleIndex)
    {
        isSwitchingBall = true;

        List<Game.Ball> ballList = game.bottles[bottleIndex].balls;

        Vector3 downPosition = bottleGraphics[bottleIndex].GetBallPosition(ballList.Count-1);

        Vector3 ballPosition = bottleGraphics[bottleIndex].GetBottleUpPosition();

        previewBall.transform.position =ballPosition;

        while (Vector2.Distance(previewBall.transform.position, downPosition) > 0.005f)
        {
            previewBall.transform.position = Vector3.MoveTowards(previewBall.transform.position, downPosition, 10 * Time.deltaTime);
            yield return null;
        }

        previewBall.gameObject.SetActive(false);

        Game.Ball b = ballList[ballList.Count-1];

        bottleGraphics[bottleIndex].SetGraphic(ballList.Count - 1, b.type);

        isSwitchingBall = false;

    }

    private bool isSwitchingBall = false;
    IEnumerator SwitchBallCoroutine(int fromBottleIndex, int toBottleIndex)
    {
        isSwitchingBall = true;
       List<Game.SwitchBallCommand> commands =  game.CheckSwitchBall(fromBottleIndex, toBottleIndex);

        if(commands.Count == 0)
        {
            Debug.Log("Can't move");
           
        }
        else
        {
            pendingBalls = commands.Count;

            previewBall.gameObject.SetActive(false);

            for(int i = 0;i<commands.Count;i++)
            {
                Game.SwitchBallCommand command = commands[i];
                Queue<Vector3> moveQueue = GetCommandPath(command);

                if(i == 0)
                {
                    moveQueue.Dequeue();
                }

                StartCoroutine(SwitchBall(command, moveQueue));
                yield return new WaitForSeconds(0.06f);
            }
            //foreach(Game.SwitchBallCommand command in commands)
            //{
            //    StartCoroutine(SwitchBall(command));
            //    yield return new WaitForSeconds(0.1f);
            //}
            while(pendingBalls > 0)
            {
                yield return null;
            }

            game.SwitchBall(fromBottleIndex,toBottleIndex);

           
        }
        selectedBottleIndex = -1;
        isSwitchingBall = false;

        if (game.CheckWinCondition())
        {
            //Debug.Log("Win 2");
            GameObject win = Instantiate(winEffect);
            StartCoroutine(SetGameComplete());
            
        }
    }
    
    int pendingBalls = 0;

    private Queue<Vector3> GetCommandPath(Game.SwitchBallCommand command)
    {
        Queue<Vector3> queueMovement = new Queue<Vector3>();

        queueMovement.Enqueue(bottleGraphics[command.fromBottleIndex].GetBallPosition(command.fromBallIndex));
        queueMovement.Enqueue(bottleGraphics[command.fromBottleIndex].GetBottleUpPosition());
        queueMovement.Enqueue(bottleGraphics[command.toBottleIndex].GetBottleUpPosition());
        queueMovement.Enqueue(bottleGraphics[command.toBottleIndex].GetBallPosition(command.toBallIndex));

        return queueMovement;
    }
    IEnumerator SwitchBall(Game.SwitchBallCommand command, Queue<Vector3> movement)
    {
        // tat graphic o vtri from
        // tao  ball o v tri from cung type
        // chuyen ball theo dung duong
        // xoa ball di chuyen bat graphic o to


        bottleGraphics[command.fromBottleIndex].SetGraphicNone(command.fromBallIndex);

        Vector3 spawnPosition = movement.Peek();

        var ballObject = Instantiate(prefabBallGraphics, spawnPosition, Quaternion.identity);

        ballObject.SetColor((command.type));

        while (movement.Count > 0 )
        {
            Vector3 target = movement.Dequeue();

            while (Vector3.Distance(ballObject.transform.position, target) > 0.005f)
            {
                ballObject.transform.position = Vector3.MoveTowards(ballObject.transform.position,target, 10 * Time.deltaTime);
                yield return null;
            }
        }

        yield return null;

        Destroy(ballObject.gameObject);

        bottleGraphics[command.toBottleIndex].SetGraphic(command.toBallIndex, command.type);

        pendingBalls--;
    }

    IEnumerator SetGameComplete()
    {
        GameManager.instance.SetGameState(GameState.LevelComplete);
        yield return new WaitForSeconds(1f);
    }
}
