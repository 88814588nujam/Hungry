using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenShatterTransition : MonoBehaviour
{
    public int fragmentSize = 32;
    public float explosionForce = 5f;
    public float transitionDelay = 1.5f;
    public string nextSceneName;
    private bool isTransitioning = false;

    public void StartTransition()
    {
        if (!isTransitioning)
            StartCoroutine(TransitionCoroutine());
    }

    private void CreateFragments(Texture2D texture)
    {
        Camera cam = Camera.main;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        for (int x = 0; x < screenSize.x; x += fragmentSize)
        {
            for (int y = 0; y < screenSize.y; y += fragmentSize)
            {
                int width = Mathf.Min(fragmentSize, (int)screenSize.x - x);
                int height = Mathf.Min(fragmentSize, (int)screenSize.y - y);

                Color[] pixels = texture.GetPixels(x, y, width, height);
                Texture2D pieceTexture = new Texture2D(width, height);
                pieceTexture.SetPixels(pixels);
                pieceTexture.Apply();

                Sprite pieceSprite = Sprite.Create(pieceTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

                GameObject fragment = new GameObject("Fragment");
                var sr = fragment.AddComponent<SpriteRenderer>();
                sr.sprite = pieceSprite;
                sr.sortingOrder = 999;

                Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(x + width / 2f, y + height / 2f, 10));
                fragment.transform.position = worldPos;

                var rb = fragment.AddComponent<Rigidbody2D>();
                var col = fragment.AddComponent<BoxCollider2D>();

                Vector2 forceDir = (worldPos - cam.transform.position).normalized;
                Vector2 randomDir = forceDir + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                rb.AddForce(randomDir.normalized * explosionForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator TransitionCoroutine()
    {
        isTransitioning = true;
        yield return new WaitForEndOfFrame();

        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        CreateFragments(screenTexture);
        yield return new WaitForSeconds(transitionDelay);

        SceneManager.LoadScene(nextSceneName);
    }
}