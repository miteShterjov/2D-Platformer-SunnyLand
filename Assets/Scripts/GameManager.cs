using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gem Collection Settings")]
    [SerializeField] private float gemsCollected;
    [Space]
    [Header("Screen Fading Settings")]
    [SerializeField] UnityEngine.UI.Image fadderImage;
    [SerializeField] private float fadeSpeed = 0.5f;
    [SerializeField] private float screenDelay = 0.5f;
    [Space]
    [Header("Respawn Settings")]
    [SerializeField] private Transform playerStartPosition;
    private GameObject player;
    private Checkpoint currentCheckpoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void AddGems()
    {
        gemsCollected++;
        print("Gems Collected: " + gemsCollected);
    }

    public void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null) currentCheckpoint.DisableCheckpoint();
        currentCheckpoint = checkpoint;
    }

    public void StartPlayerRespawnCo()
    {
        StartCoroutine(RespawnPlayerCoroutine());
        print("coroutine started from game manager.");
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        yield return StartCoroutine(FadeRoutine(1));
        RespawnPlayerAtCheckpoint();
        player.gameObject.SetActive(true);
        yield return StartCoroutine(FadeRoutine(0));
    }

    private void RespawnPlayerAtCheckpoint()
    {
        print("Respawning player in correct position...");
        if (currentCheckpoint != null) player.transform.position = currentCheckpoint.transform.position;
        else player.transform.position = playerStartPosition.position;
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        while (!Mathf.Approximately(fadderImage.color.a, targetAlpha))
        {
            float alpha = Mathf.MoveTowards(fadderImage.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            fadderImage.color = new Color(fadderImage.color.r, fadderImage.color.g, fadderImage.color.b, alpha);
            yield return new WaitForSeconds(screenDelay);
        }
    }

}
