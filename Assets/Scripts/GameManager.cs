using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static bool lastFail;
    public int leftPlayerState = 0, rightPlayerState = 0, maxState = 4;
    public Snake snake;
    public BonusCotroller bonusSpawner;
    public static Vector2 BottomLeft;
    public static Vector2 TopRight;
    private bool isPaused = false;
    private List<Orange> orange;
    private List<Purple> purple;

    [SerializeField] private GameObject dim;
    [SerializeField] private GameObject pauseOverlay;


    private Vector2 snakeVelocityOnPause;
    private float pauseDuration;
    private Paddle rightPaddle, leftPaddle;

    private bool isRightPaddle = true;
    private bool isLeftPaddle = false;
//    Color32 color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);  //White Color in Hex
    Color orangeStart = new Color32(255,121, 0, 255); //FF7900
    Color orangeEnd = new Color32(255, 150, 0, 255);
    Color pinkStart = new Color32(255, 0, 255, 255); //FF00FF
    Color pinkEnd = new Color32(128, 38, 188, 255);
    Color greenStart = new Color32(0, 255, 12, 255); //00FF0C
    Color greenEnd = new Color32(0, 255, 0, 200);

    private float duration = 0.8f;

    private Vector3 cameraInitialPos;
    [SerializeField] private float shakeMagnitude = 0.05f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private Camera camera;


    void Awake()
    {
        QualitySettings.antiAliasing = 0;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    public void Shake()
    {
        cameraInitialPos = camera.transform.position;
        InvokeRepeating("StartCameraShaking", 0f, 0.005f);
        Invoke("StopCameraShaking", shakeDuration);
    }


    void StartCameraShaking()
    {
        float cameraShakingOffsetX = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        float cameraShakingOffsetY = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        Vector3 cameraIntermediatePosition = camera.transform.position;
        cameraIntermediatePosition.x += cameraShakingOffsetX;
        cameraIntermediatePosition.y += cameraShakingOffsetY;
        camera.transform.position = cameraIntermediatePosition;
    }

    void StopCameraShaking()
    {
        CancelInvoke("StartCameraShaking");
        camera.transform.position = cameraInitialPos;
    }
    // Start is called before the first frame update
    void Start()
    {
        dim.SetActive(false);
        pauseOverlay.SetActive(false);
        SoundsManager.Instance.PlayMainGameBGM();
        BottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        TopRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        
        snake = Instantiate(Resources.Load<Snake>("SnakeHead"));
        snake.GetComponent<Snake>().gameManager = this;
        rightPaddle =  Instantiate(Resources.Load<Paddle>("PaddleMainBrickR"));
        leftPaddle =  Instantiate(Resources.Load<Paddle>("PaddleMainBrickL"));
        rightPaddle.Init(isRightPaddle);
        leftPaddle.Init(isLeftPaddle);
        leftPaddle.gameObject.tag = "LPaddle";
        rightPaddle.gameObject.tag = "RPaddle";

        float size = 0.67f;
        orange = new List<Orange>();
        for (int i = 0; i < 4; i++)
        {
            Orange orange1 = Instantiate(Resources.Load<Orange>("PaddleBrickOrange"));
            orange1.transform.position += size * i * (new Vector3(-1, 1, 0));
            orange.Add(orange1);
        }
        
        purple = new List<Purple>();
        for (int i = 0; i < 4; i++)
        {
            Purple purple1 = Instantiate(Resources.Load<Purple>("PaddleBrickPurple"));
            purple1.transform.position += size * i * (new Vector3(1, 1, 0));
            purple.Add(purple1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {  
                pauseOverlay.GetComponent<Animator>().SetBool("endPause", true);
                isPaused = false;

                //                Unpause();
            }
            else
            {
                isPaused = true;
                Pause();
            }
        }

        // manage colors for paddles :

        float lerp = Mathf.PingPong(Time.time, duration) / duration;

        if (rightPaddle != null)
        {
            rightPaddle.GetComponent<Renderer>().material.color = Color.Lerp(pinkStart, pinkEnd, lerp); // color right paddle
            Renderer[] rightChildrenRenderer = rightPaddle.GetComponentsInChildren<Renderer>();
            foreach (var r in rightChildrenRenderer)
            {
                r.material.color = Color.Lerp(pinkStart, pinkEnd, lerp); // color the children of right paddle the same colors
            }
        }
        if (leftPaddle != null)
        {
            leftPaddle.GetComponent<Renderer>().material.color = Color.Lerp(orangeStart, orangeEnd, lerp);
            Renderer[] leftChildrenRenderer = leftPaddle.GetComponentsInChildren<Renderer>();
            foreach (var r in leftChildrenRenderer)
            {
                r.material.color = Color.Lerp(orangeStart, orangeEnd, lerp); // color the children of right paddle the same colors
            }
        }
        

        //manage snake colors:
        Color color1, color2;
        int idx = 1;
        if (snake.lastTarget == 1) //if snake hit the right paddle last
        {
            color1 = pinkStart;
            color2 = pinkEnd;
        }
        else if(snake.lastTarget == 2)
        {
            color1 = orangeStart;
            color2 = orangeEnd;
        }
        else
        {
            color1 = greenStart;
            color2 = greenEnd;
        }
        snake.GetComponent<Renderer>().material.color = Color.Lerp(color1, color2, lerp); // color like right paddle
        foreach (var bodyPart in snake.bodyParts)
        {
            Color reduceAlpha = new Color32(0, 0, 0, 40);
            if (idx <= 4)
            {
                color1 -= reduceAlpha;
                color2 -= reduceAlpha;
            }
            bodyPart.GetComponent<Renderer>().material.color = Color.Lerp(color1, color2, lerp); 
            idx++;
        }
    }
    public void IncreasePaddle(bool playerSide)
    {
        Debug.Log("Paddle increase!");
        Paddle paddle;
        string sideTop;
        string sideBottom;
        int state;
        if (playerSide)
        {
            paddle = rightPaddle;
            state = rightPlayerState;
            sideTop = "PaddleBrickRTop";
            sideBottom = "PaddleBrickRBottom";
        }
        else
        {
            paddle = leftPaddle;
            state = leftPlayerState;
            sideTop = "PaddleBrickLTop";
            sideBottom = "PaddleBrickLBottom";
        }

        Vector2 topPos, bottomPos;
        
        PaddleBrick addedTop = Instantiate(Resources.Load<PaddleBrick>(sideTop));
        PaddleBrick addedBottom = Instantiate(Resources.Load<PaddleBrick>(sideBottom));
        topPos = new Vector2(paddle.transform.position.x, paddle.transform.position.y + (paddle.transform.localScale.y *2  + state * addedTop.transform.localScale.y /2));
        bottomPos = new Vector2(paddle.transform.position.x, paddle.transform.position.y - (paddle.transform.localScale.y *2 + state * addedBottom.transform.localScale.y/2));
        addedTop.Init(playerSide, paddle);
        addedBottom.Init(playerSide, paddle);
        addedTop.transform.position = topPos;
        addedBottom.transform.position = bottomPos;
        if (paddle.isRight)
        {
            addedBottom.gameObject.tag = "RPaddle";
            addedTop.gameObject.tag = "RPaddle";
        }
        else
        {
            addedBottom.gameObject.tag = "LPaddle";
            addedTop.gameObject.tag = "LPaddle";
        }
        addedBottom.transform.SetParent(paddle.transform);
        addedTop.transform.SetParent(paddle.transform);
        addedBottom.GetComponent<Animator>().SetTrigger("born");
        addedTop.GetComponent<Animator>().SetTrigger("born");
    }

    public void PointHandler(bool playerMiss)
    {
        Shake();
        // Returns the paddles to original position.
        rightPaddle.transform.position = new Vector2(GameManager.TopRight.x, 0) + (Vector2.left * transform.localScale.x);
        leftPaddle.transform.position = new Vector2(GameManager.BottomLeft.x, 0) + (Vector2.right * transform.localScale.x);
        if (playerMiss)
        {
            Purple purple1;
            purple1 = purple[purple.Count - 1];
            purple.RemoveAt(purple.Count - 1);
            purple1.gameObject.SetActive(false);
            Destroy(purple1.gameObject);
            
            if (rightPlayerState + 1 >= maxState)
            {
                EndGame(true);
                //goto left player won screen
            }
            else
            {
                IncreasePaddle(playerMiss);
                rightPlayerState += 1;
            }
           
        }
        else
        {
            Orange orange1;
            orange1 = orange[orange.Count - 1];
            orange.RemoveAt(orange.Count - 1);
            orange1.gameObject.SetActive(false);
            Destroy(orange1.gameObject);
            
            if (leftPlayerState + 1 >= maxState)
            {
                EndGame(false);
                //goto right player won screen
//                Time.timeScale = 0;
            }
            else
            {
                IncreasePaddle(playerMiss);
                leftPlayerState += 1;
            }
            
        }

        
        
    }

    public void EndGame(bool isRight)
    {
        //Stop everything
        DisableInput();
        KillSnake();
        StopBonusProduction();
        //Destroy the paddle that lost:
        StaticWinner.rightPlayerLost = isRight;
            Paddle toDestroy;
        toDestroy = isRight ? rightPaddle : leftPaddle;
        toDestroy.playerPaddle.velocity = Vector2.zero; // no movement befor destruction
        for (int i = 0; i < toDestroy.transform.childCount; i++)
        {
            Destroy(toDestroy.transform.GetChild(i).gameObject);
        }
        toDestroy.GetComponent<Animator>().SetTrigger("isDead");
        SoundsManager.Instance.PlayLoseRoundSound();
        SoundsManager.Instance.PlayUsedLastLifeSound();
        Destroy(toDestroy);
    }

    private void DisableInput()
    {
        leftPaddle.InputEnabled = false;
        rightPaddle.InputEnabled = false;
    }

    private void EnableInput()
    {
        leftPaddle.InputEnabled = true;
        rightPaddle.InputEnabled = true;
    }


    private void KillSnake()
    {
        foreach (var snakePart in snake.bodyParts)
        {
            snakePart.gameObject.SetActive(false);
        }
    }

    private void StopSnake()
    {
        pauseDuration = snake.increaseSpeedInterval - Time.time;
        if (snake.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
        {
            snakeVelocityOnPause = snake.GetComponent<Rigidbody2D>().velocity;
        }
        snake.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        foreach (var snakePart in snake.bodyParts)
        {
            snakePart.gameObject.SetActive(false);
        }
    }

    private void StartSnake()
    {
        pauseDuration = Time.time + pauseDuration;
        foreach (var snakePart in snake.bodyParts)
        {
            snakePart.gameObject.SetActive(true);
        }
        snake.GetComponent<Rigidbody2D>().velocity = snakeVelocityOnPause;
        snake.increaseSpeedInterval = pauseDuration;
    }


    private void StopBonusProduction()
    {
        bonusSpawner.gameObject.SetActive(false);
    }

    private void StartBonusProduction()
    {
        bonusSpawner.gameObject.SetActive(true);
    }

    void Pause()
    {
        StopBonusProduction();
        DisableInput();
        StopSnake();
        dim.SetActive(true);
        pauseOverlay.SetActive(true);
    }

    public void Unpause()
    {
        StartBonusProduction();
        EnableInput();
        StartSnake();
        dim.SetActive(false);
        pauseOverlay.SetActive(false);
    }
}
