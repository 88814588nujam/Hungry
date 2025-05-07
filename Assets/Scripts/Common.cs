using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Common : MonoBehaviour
{
    [Header("Common Control")]
    public Text scoreText;
    public Text highestScoreText;
    public Text nextText;
    public float blinkInterval = 0.5f;
    public string sceneName;
    private ScreenShatterTransition screenShatterTransitionInstance;

    void Start()
    {
        screenShatterTransitionInstance = FindFirstObjectByType<ScreenShatterTransition>();
        float lastScore = PlayerPrefs.GetFloat("lastScore");
        if (sceneName.Equals("menu"))
            scoreText.text = "Last Score : " + lastScore.ToString();
        else
        { 
            if (lastScore == 0) scoreText.text = "I didn't eat alone this time.";
            else scoreText.text = "I ate " + lastScore.ToString() + " people this time.";
        }
        float highestScore = PlayerPrefs.GetFloat("HighestScore");
        highestScoreText.text = "Highest Score : " + highestScore.ToString();
        StartCoroutine(Blink());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Quit();
        else 
            if (Input.anyKeyDown)
                screenShatterTransitionInstance.StartTransition();
    }

    private void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    IEnumerator Blink()
    {
        while (true)
        {
            nextText.enabled = !nextText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}