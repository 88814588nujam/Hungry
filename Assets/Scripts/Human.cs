using UnityEngine;

public class Human : MonoBehaviour
{
    private Animator animator;
    private bool executeOver = false;
    private GameControl gameControlInstance;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        gameControlInstance = FindFirstObjectByType<GameControl>();

        int randDirection = Random.Range(0, 2);
        int randPlusMinus = Random.Range(0, 2);
        if (randDirection == 0) {
            animator.SetFloat("Horizontal", randPlusMinus == 0 ? -1 : 1);
            animator.SetFloat("Vertical", 0);
        }
        else {
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", randPlusMinus == 0 ? -1 : 1);
        }
    }

    void Update()
    {
        if (animator.GetBool("IsDead") && !executeOver)
        {
            executeOver = true;
            Invoke("DestroyObject", 1.0f);
        }
        bool isGameOver = gameControlInstance.isGameOver;
        if(isGameOver)
            animator.SetBool("IsOver", true);
    }

    private void DestroyObject() {
        gameControlInstance.CreateHuman();
        Destroy(gameObject);
    }
}