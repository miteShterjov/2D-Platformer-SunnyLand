using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] UnityEngine.UI.Image fadderImage;
    [SerializeField] private float fadeSpeed = 0.5f;
    [SerializeField] private float screenDelay = 0.5f;
    private GameObject player;
    private Transform playerStartPosition;
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
        if (player == null) Debug.LogError("Player not found in the scene. Please ensure the player has the 'Player' tag assigned.");
        playerStartPosition = player.transform;
        if (playerStartPosition == null) Debug.LogError("Player start position is not set properly.");
    }

    public void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null) currentCheckpoint.DisableCheckpoint();
        currentCheckpoint = checkpoint;
    }

    public void StartPlayerRespawnCo()
    {
        StartCoroutine(RespawnPlayerCoroutine());
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
