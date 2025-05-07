using System.Collections.Generic;
using UnityEngine;

public class HeadZombie : MonoBehaviour
{
    [Header("Zombie Speed")]
    public float moveSpeed = 2.0f;
    public float speedIncrease = 0.5f;
    public float speedMax = 6.0f;
    public int eatenCountIncrease = 3;

    [Space(10)]
    [Header("Head Zombie")]
    public AudioSource attackSource;
    public AudioSource deadSource;

    [Space(10)]
    [Header("Body Zombie")]
    public GameObject bodyZombiePrefab;
    public float moveGap = 0.5f;
    public float hitGap = 0.3f;

    private Vector2 moveDirection = Vector2.down;
    private List<GameObject> bodyZombieParts = new List<GameObject>();
    private List<Vector3> positionsHistory = new List<Vector3>();
    private int humanEaten = 0;
    private Animator animator;
    private bool isGameOver = false;
    private GameControl gameControlInstance;
    private ScreenShatterTransition screenShatterTransitionInstance;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        gameControlInstance = FindFirstObjectByType<GameControl>();
        screenShatterTransitionInstance = FindFirstObjectByType<ScreenShatterTransition>();
        positionsHistory.Add(this.transform.position);
    }

    void Update()
    {
        SpriteRenderer headZombieRenderer = this.GetComponent<SpriteRenderer>();
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            if (moveDirection != Vector2.down)
            {
                headZombieRenderer.sortingOrder = 10;
                moveDirection = Vector2.up;
            }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            if (moveDirection != Vector2.up)
            {
                headZombieRenderer.sortingOrder = bodyZombieParts.Count + 999;
                moveDirection = Vector2.down;
            }
                
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            if (moveDirection != Vector2.right)
            {
                headZombieRenderer.sortingOrder = bodyZombieParts.Count + 999;
                moveDirection = Vector2.left;
            }
        
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            if (moveDirection != Vector2.left)
            {
                headZombieRenderer.sortingOrder = bodyZombieParts.Count + 999;
                moveDirection = Vector2.right;
            }
        
    }

    void FixedUpdate()
    {
        if (!isGameOver) { 
            MoveHeadZombie();
            MoveBodyZombie();
            CheckHitBodyZombie();
        }
    }

    private void MoveHeadZombie()
    {
        Vector3 moveVector = new Vector3(moveDirection.x, moveDirection.y, 0);
        this.transform.position += moveVector * moveSpeed * Time.fixedDeltaTime;
        animator.SetFloat("Horizontal", moveVector.x);
        animator.SetFloat("Vertical", moveVector.y);
        animator.SetFloat("Speed", moveVector.magnitude);
        positionsHistory.Insert(0, this.transform.position);
    }

    private void MoveBodyZombie()
    {
        for (int i = 0; i < bodyZombieParts.Count; i++)
        {
            Transform bodyPos = bodyZombieParts[i].transform;
            Vector3 prevPos = positionsHistory[Mathf.Min(Mathf.RoundToInt((i + 1) * moveGap / (moveSpeed * Time.fixedDeltaTime)), positionsHistory.Count - 1)];
            Vector3 moveDirection = prevPos - bodyPos.position;
            bodyPos.position += moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
            Animator bodyZombieAnimator = bodyZombieParts[i].GetComponent<Animator>();
            bodyZombieAnimator.SetFloat("Horizontal", moveDirection.x);
            bodyZombieAnimator.SetFloat("Vertical", moveDirection.y);
            bodyZombieAnimator.SetFloat("Speed", moveDirection.magnitude);          
            SpriteRenderer bodyZombieRenderer = bodyZombieParts[i].GetComponent<SpriteRenderer>();
            if (moveDirection.y > 0)
                bodyZombieRenderer.sortingOrder = (i + 1) + 10;
            else
                bodyZombieRenderer.sortingOrder = (bodyZombieParts.Count - i) + 10;
        }
        while (positionsHistory.Count > (bodyZombieParts.Count + 1) * 50)
            positionsHistory.RemoveAt(positionsHistory.Count - 1);
    }

    private void CreateZombie()
    {
        GameObject bodyZombiePart = Instantiate(bodyZombiePrefab);
        bodyZombiePart.transform.position = bodyZombieParts.Count > 0 ? bodyZombieParts[bodyZombieParts.Count - 1].transform.position : this.transform.position;
        bodyZombieParts.Add(bodyZombiePart);
        humanEaten++;
        gameControlInstance.scoreText.text = "Score : " + humanEaten.ToString();
        if (humanEaten % eatenCountIncrease == 0 && moveSpeed < speedMax)
            moveSpeed += speedIncrease;
    }

    private void CheckHitBodyZombie()
    {
        for (int i = 2; i < bodyZombieParts.Count; i++)
            if (Vector2.Distance(transform.position, bodyZombieParts[i].transform.position) < hitGap)
                GameOver();
    }

    private void GameOver()
    {
        if (!isGameOver)
        {
            deadSource.Play();
            animator.SetBool("IsDead", true);
            isGameOver = true;
            gameControlInstance.isGameOver = true;
            for (int i = 0; i < bodyZombieParts.Count; i++)
            {
                Animator bodyZombieAnimator = bodyZombieParts[i].GetComponent<Animator>();
                bodyZombieAnimator.SetBool("IsDead", true);
            }
            Invoke("StartTransition", 1.0f);
            PlayerPrefs.SetFloat("lastScore", humanEaten);
            float highestScore = PlayerPrefs.GetFloat("HighestScore");
            if (highestScore < humanEaten)
                PlayerPrefs.SetFloat("HighestScore", humanEaten);
        }
    }

    private void StartTransition()
    {
        screenShatterTransitionInstance.StartTransition();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Human")
        {
            attackSource.Play();
            animator.SetTrigger("Attack");
            gameControlInstance.isBloodDisplay = true;
            Animator humanAnimator = collision.gameObject.GetComponent<Animator>();
            humanAnimator.SetBool("IsDead", true);
            CreateZombie();
        }
        else if (collision.gameObject.tag == "Trap")
            GameOver();
    }
}