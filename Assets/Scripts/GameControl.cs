using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    [Header("Game Control")]
    public bool isGameOver = false;
    public Text scoreText;
    public Text highestScoreText;

    [Header("Create Human")]
    public Transform humanPrefab;
    public Vector2 displayArea;

    [Header("Eat Blood")]
    public Image imageToFade;
    public float fadeDuration = 0.4f;
    public bool isBloodDisplay = false;

    void Start()
    {
        float highestScore = PlayerPrefs.GetFloat("HighestScore");
        highestScoreText.text = "Highest Score : " + highestScore.ToString();
        CreateHuman();
    }

    void Update()
    {
        if (isBloodDisplay)
        {
            StartCoroutine(Fade());
            isBloodDisplay = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Quit();
    }

    public void CreateHuman()
    {
        float createX, createY;
        createX = GetRandPosition(displayArea.x);
        createY = GetRandPosition(displayArea.y);
        Transform appleInstance = Instantiate(humanPrefab, new Vector3(createX, createY, 0), Quaternion.identity);
    }

    private float GetRandPosition(float limitPosition) { 
        return Random.Range(-limitPosition / 2, limitPosition / 2);
    }

    private void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); // 在打皝E蟮挠蜗分型顺丒
        #endif
    }

    IEnumerator Fade()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            imageToFade.color = new Color(imageToFade.color.r, imageToFade.color.g, imageToFade.color.b, Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration));
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            imageToFade.color = new Color(imageToFade.color.r, imageToFade.color.g, imageToFade.color.b, Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration));
            yield return null;
        }
    }
}