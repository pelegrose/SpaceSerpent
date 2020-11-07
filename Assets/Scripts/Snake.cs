using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Snake : MonoBehaviour
{
    [SerializeField] private int firstBodyPartToCollideWithI = 7;
    private int _bodySize;
    private Transform _cur;
    private Vector2 _direction;
    public float increaseSpeedInterval;
    private float _initialSpeed;
    private Transform _prev;
    private float _radius;
    private Rigidbody2D _snakeHead;
    public List<Transform> currentBodyParts;
    public List<Transform> bodyParts = new List<Transform>();
    public SnakeBody bodyPrefab;
    public GameManager gameManager;
    private readonly bool leftPlayer = false;
    public int lastTarget = 0;
    private readonly bool rightPlayer = true;
    public float speed;
    private int rightPaddle;
    private int leftPaddle;

    private bool _reset;
    private float _moveDelay; 
    private Vector2 _resetPos = Vector2.zero;

    [SerializeField] private float animationDelayBetweenParts;

    enum Direction
    {
        Any,
        Right,
        Left
    };

    // Start is called before the first frame update
    private void Start()
    {
        _reset = true;
        increaseSpeedInterval = Time.time + 3f;
        _snakeHead = GetComponent<Rigidbody2D>();
        _direction = RandomDirection((int)Direction.Any).normalized;
        _radius = transform.localScale.x / 2;
        speed = 500f;
        _initialSpeed = 500f;
        _bodySize = 0;
        bodyParts.Add(transform);
        _moveDelay = Time.time + 3f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_reset)
        {
            foreach (var bodyPart in bodyParts)
            {
                bodyPart.position = Camera.main.gameObject.transform.position; // Hides the snake for the reset period
            }
            if (Time.time >= _moveDelay)
            {
                foreach (var bodyPart in bodyParts)
                {
                    bodyPart.position = _resetPos;
                }
                _snakeHead.velocity = _direction * Time.deltaTime * speed;
                _reset = false;
                _moveDelay = Time.time + 1f;
            }
        }
        else
        {
            var velocity = _snakeHead.velocity;
            var angle = Vector2.SignedAngle(velocity, Vector2.up);
            _snakeHead.transform.rotation = Quaternion.Euler(0, 0, -angle);
            IncreaseSpeed();
            MoveSnake();
            CheckSnakePushedOut();
        }
    }


    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("LPaddle") || coll.collider.CompareTag("RPaddle"))
        {
            SoundsManager.Instance.PlayRandomPaddleHitSound();
            var initialSpeed = _snakeHead.velocity.magnitude;
            var paddleVelocity = coll.collider.attachedRigidbody.velocity;
            Vector2 vel;
            vel.x = _snakeHead.velocity.x;
            vel.y = _snakeHead.velocity.y + paddleVelocity.y / 5;
            _snakeHead.velocity = PerpSnake(paddleVelocity, _snakeHead.velocity, vel) * Time.deltaTime * speed;
            if (coll.collider.tag == "RPaddle")
            {
                rightPaddle = 1;
                lastTarget = rightPaddle;
            }
            else
            {
                leftPaddle = 2;
                lastTarget = leftPaddle;
            }
        }

        if (coll.collider.CompareTag("TBWall"))
        {
            SoundsManager.Instance.PlayRandomWallHitSound();
        }

    }


    private void ResetSnake(bool lastFail)
    {
        _reset = true;
        _moveDelay = Time.time + 1f;
        speed = _initialSpeed;
//        Debug.Log("BodtParts count Reset: " + bodyParts.Count);
//        _snakeHead.velocity = Vector2.zero;
        if (lastFail)
        {
            _direction = RandomDirection((int) Direction.Left).normalized;
//            GameManager.lastFail = false;
        }
        else
        {
            _direction = RandomDirection((int) Direction.Right).normalized;
//            GameManager.lastFail = true;
        }
//        _snakeHead.velocity = _direction * Time.deltaTime * speed;
        lastTarget = 0;
    }


    private void MoveSnake()
    {
        for (var i = 1; i < bodyParts.Count; i++)
        {
            _cur = bodyParts[i];
            _prev = bodyParts[i - 1];
            _cur.rotation = _prev.rotation;
            Vector2 newPos = _prev.position;
            _cur.position = Vector3.Lerp(_cur.position, newPos, 0.3f);
        }
    }

    private void IncreaseSpeed()
    {
        if (Time.time > increaseSpeedInterval)
        {
            speed += 30f;
            var normVelocity = _snakeHead.velocity.normalized;
            _snakeHead.velocity = normVelocity * Time.deltaTime * speed;
            increaseSpeedInterval += 2f;
        }
    }



    private void AddBodyPart()
    {
        var angle = bodyParts[_bodySize].eulerAngles.z;
        var newBodyPart = Instantiate(Resources.Load<SnakeBody>("SnakeBody"),
            bodyParts[_bodySize].position,
            bodyParts[_bodySize].rotation);
        _bodySize += 1;
        newBodyPart.Init(_bodySize);
        bodyParts.Add(newBodyPart.transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bonus"))
        {
            Debug.Log("You Hit The Bonus!");
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            AddBodyPart();
            SoundsManager.Instance.PlayEatSound();
            StartCoroutine(animateSnake());
        }
    }

    public IEnumerator animateSnake() //Animation Coroutine
    {
        float delay = animationDelayBetweenParts * Time.deltaTime;
        currentBodyParts = new List<Transform>(bodyParts);
        foreach (var bodyPart in currentBodyParts)
        {
            yield return new WaitForSeconds(delay);
            if (bodyPart != null)
            {
                bodyPart.GetComponent<Animator>().SetTrigger("ate");
            }
            delay += 0.7f * Time.deltaTime;
        }
    }

    // Spawns the snake in a random direction according to 
    // the int direction: 0 is Any direction
    // 1 is Right
    // 2 is Left
    private Vector2 RandomDirection(int direction)
    {
        var degree = 0f;
        var coinFlip = Random.Range(0, 2);;
        switch (direction)
        {
            case (int) Direction.Any:
                if (coinFlip == 1)
                {
                    degree = Random.Range(30f, 150f);
                }
                else
                {
                    degree = Random.Range(210f, 300f);
                }
                break;
            case (int) Direction.Right:
                degree = Random.Range(30f, 150f);
                break;
            case (int) Direction.Left:
                degree = Random.Range(210f, 300f);;
                break;
            default:
                Debug.Log("Bad direction.");
                break;
        }
        return DegreeToVector2(degree);
    }

    // Checks if the snake got pushed out of the game 
    // by one of the paddles. If so, the snake is reset.
    private void CheckSnakePushedOut()
    {
        float yPos = Mathf.Abs(transform.position.y);
        if ((yPos > GameManager.TopRight.y + 2 && transform.position.x > 0) ||
            transform.position.x > GameManager.TopRight.x + 2)
        {
            SoundsManager.Instance.PlayLoseRoundSound();
            gameManager.PointHandler(rightPlayer);
            ResetSnake(rightPlayer);
        }

        if ((yPos > GameManager.TopRight.y + 2 && transform.position.x < 0) ||
            transform.position.x < GameManager.BottomLeft.x - 2)
        {
            SoundsManager.Instance.PlayLoseRoundSound();
            gameManager.PointHandler(leftPlayer);
            ResetSnake(leftPlayer);
        }
    }

    private Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
    }

    private Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    private float VectorToDegree(Vector2 vector)
    {
        var degree = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        return Mathf.Repeat(degree, 360f);
    }

    // Makes sure the snake wont simply collide with itself upon a Paddle hit.
    // Keeps the return angle 20 degrees off the original velocity vector.
    private Vector2 PerpSnake(Vector2 paddleVel, Vector2 preImpactVel, Vector2 afterImpactVel)
    {
        var returnAngle = VectorToDegree(afterImpactVel);
        Vector2 velocity = LimitReturnAngle(afterImpactVel, returnAngle);
        returnAngle = VectorToDegree(velocity);
        var snakeAngle = 360f - transform.eulerAngles.z;
        var angleDiff = snakeAngle - returnAngle;
        if (angleDiff > 160f && angleDiff <= 200f)
        {
            if (paddleVel.y > 0f) // trying to move the snake up
            {
                if (returnAngle - (180f + 20f - angleDiff)  >= 20f)
                {
                    velocity = DegreeToVector2(returnAngle - (180f + 20f - angleDiff));
                }
                else
                {
                    velocity = DegreeToVector2(returnAngle + (-180f + 20f + angleDiff));
                }
            }
            else if (paddleVel.y < 0f) // trying to move the snake down
            {
                if (returnAngle + (-180f + 20f + angleDiff) <= 160f)
                {
                    velocity = DegreeToVector2(returnAngle + (-180f + 20f + angleDiff));
                }
                else
                {
                    velocity = DegreeToVector2(returnAngle - (180f + 20f - angleDiff));
                }
            }
            else // The paddle is not moving.
            {
                if (transform.position.y > 0) //  Move down.
                {
                    velocity = DegreeToVector2(returnAngle + (-180f + 20f + angleDiff));
                }
                else // Move up.
                {
                    velocity = DegreeToVector2(returnAngle - (180f + 20f - angleDiff));
                }
            }
        }
        else if (angleDiff < -160f && angleDiff >= -200f)
        {
            if (paddleVel.y > 0f) // trying to move the snake up
            {
                if (returnAngle + (180f + 20f + angleDiff)  <= 340f)
                {
                    velocity = DegreeToVector2(returnAngle + (180f + 20f + angleDiff));
                }
                else
                {
                    velocity = DegreeToVector2(returnAngle - (-180f + 20f - angleDiff));
                }
            }
            else if (paddleVel.y < 0f) // trying to move the snake down
            {
                if (returnAngle - (-180f + 20f - angleDiff) >= 200f)
                {
                    velocity = DegreeToVector2(returnAngle - (-180f + 20f - angleDiff));
                }
                else
                {
                    velocity = DegreeToVector2(returnAngle + (180f + 20f + angleDiff));
                }
            }
            else
            {
                if (transform.position.y < 0) // Move up
                {
                    velocity = DegreeToVector2(returnAngle + (180f + 20f + angleDiff));
                }
                else // Move down
                {
                    velocity = DegreeToVector2(returnAngle - (-180f + 20f - angleDiff));
                }
            }
        }
         return velocity.normalized;
    }

    // Limits the return angle to not be stright up or down
    private Vector2 LimitReturnAngle(Vector2 afterImpactVel, float returnAngle)
    {
        Vector2 vec;
        if (returnAngle >= 0f && returnAngle < 20f)
        {
            vec = DegreeToVector2(20f);
        }
        else if (returnAngle < 360f && returnAngle > 340f)
        {
            vec = DegreeToVector2(340f);
        }
        else if (returnAngle < 200f && returnAngle >= 180f)
        {
            vec = DegreeToVector2(200f);
        }
        else if (returnAngle < 180f && returnAngle > 160f)
        {
            vec = DegreeToVector2(160f);
        }
            else
        {
            vec = afterImpactVel;
        }

            return vec.normalized;
    }

    public void ResetAfterCollision(int cutOff)
    {
        //SnakeBody.isTriggered = false;
        Transform bp;
        for (var i = bodyParts.Count - 1; i >= cutOff; i--)
        {
            bp = bodyParts[i];
            bodyParts.RemoveAt(i);
            _bodySize -= 1;
            bp.gameObject.SetActive(false);
            Destroy(bp.gameObject);
        }
		SnakeBody.isTriggered = false;

        if (lastTarget == 1) // Right paddle
        {
            SoundsManager.Instance.PlayLoseRoundSound();
            gameManager.PointHandler(rightPlayer);
            ResetSnake(rightPlayer);
        }
        else if (lastTarget == 2) // Left paddle
        {
            SoundsManager.Instance.PlayLoseRoundSound();
            gameManager.PointHandler(leftPlayer);
            ResetSnake(leftPlayer);
        }
    }
}