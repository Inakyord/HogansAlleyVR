using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class npcManager : MonoBehaviour
{
    public List<GameObject> npcs;
    public List<GameObject> places;
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Audio Settings")]
    public AudioSource musicSource;   // Drag the 1st AudioSource here
    public AudioSource sfxSource;     // Drag the 2nd AudioSource here
    public AudioClip bgMusicClip;     // Drag your music file here
    public AudioClip enemyHitClip;    // Drag "Boom" sound here
    public AudioClip innocentHitClip; // Drag "Ouch" sound here

    private List<GameObject> allies = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();
    private int currentPoints = 0;
    
    // DIFFICULTY SETTINGS
    private float maxTime = 40f; 
    private float timeLeft;
    
    private bool isGameOver = false;

    // Track NPCs currently animating
    private HashSet<GameObject> animatingNpcs = new HashSet<GameObject>();

    void Start()
    {
        timeLeft = maxTime;

        // --- AUDIO SETUP ---
        if (musicSource != null && bgMusicClip != null)
        {
            musicSource.clip = bgMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
        // -------------------

        foreach (GameObject npc in npcs)
        {
            if (npc.name.StartsWith("Innocent")) allies.Add(npc);
            else enemies.Add(npc);

            Button btn = npc.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnNpcClicked(npc));
            }
            npc.SetActive(false);
        }

        UpdateScoreUI();
        StartCoroutine(MainGameCycle());
    }

    void Update()
    {
        if (isGameOver) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            timeLeft = 0;
            EndGame();
        }
        if (timerText != null) timerText.text = "Time: " + Mathf.CeilToInt(timeLeft).ToString();

        // BILLBOARD LOGIC
        if (Camera.main != null)
        {
            foreach (GameObject npc in npcs)
            {
                if (npc.activeInHierarchy && !animatingNpcs.Contains(npc))
                {
                    npc.transform.rotation = Camera.main.transform.rotation;
                }
            }
        }
    }

    IEnumerator MainGameCycle()
    {
        while (!isGameOver)
        {
            float difficultyFactor = timeLeft / maxTime; 
            float currentRestTime = Mathf.Lerp(1.0f, 2.5f, difficultyFactor);
            
            yield return new WaitForSeconds(currentRestTime);
            
            if (isGameOver) break;
            yield return StartCoroutine(SpawnGroupRoutine());
        }
    }

    IEnumerator SpawnGroupRoutine()
    {
        List<GameObject> availableEnemies = enemies.Where(e => !e.activeInHierarchy).ToList();
        List<GameObject> availableAllies = allies.Where(a => !a.activeInHierarchy).ToList();
        List<GameObject> availablePlaces = places.OrderBy(x => Random.value).Take(3).ToList();

        if (availableEnemies.Count >= 2 && availableAllies.Count >= 1 && availablePlaces.Count >= 3)
        {
            List<GameObject> selectedEnemies = availableEnemies.OrderBy(x => Random.value).Take(2).ToList();
            GameObject selectedAlly = availableAllies[Random.Range(0, availableAllies.Count)];
            List<GameObject> group = new List<GameObject>(selectedEnemies) { selectedAlly };

            // 1. SPAWN
            for (int i = 0; i < 3; i++)
            {
                GameObject npc = group[i];
                npc.transform.position = availablePlaces[i].transform.position;
                
                Button btn = npc.GetComponent<Button>();
                if (btn != null) btn.interactable = true;

                StartCoroutine(AnimateSpawn(npc, npc.transform.position));
            }

            float difficultyFactor = timeLeft / maxTime;
            float roundDuration = Mathf.Lerp(2.0f, 5.0f, difficultyFactor);

            // 2. WAIT
            float elapsed = 0f;
            while (elapsed < roundDuration)
            {
                if (isGameOver) yield break; 
                elapsed += Time.deltaTime;
                yield return null; 
            }

            // 3. DESPAWN
            yield return StartCoroutine(AnimateGroupDespawn(group));
        }
    }

    IEnumerator AnimateSpawn(GameObject npc, Vector3 targetPos)
    {
        animatingNpcs.Add(npc);
        npc.SetActive(true);

        float duration = 0.3f;
        float timer = 0f;
        Vector3 finalScale = Vector3.one;
        Vector3 startScale = Vector3.zero;
        Vector3 startPos = targetPos + (Vector3.down * 0.5f);

        if (Camera.main != null) npc.transform.rotation = Camera.main.transform.rotation;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            progress = Mathf.SmoothStep(0, 1, progress);

            npc.transform.position = Vector3.Lerp(startPos, targetPos, progress);
            npc.transform.localScale = Vector3.Lerp(startScale, finalScale, progress);
            yield return null;
        }

        npc.transform.position = targetPos;
        npc.transform.localScale = finalScale;
        animatingNpcs.Remove(npc);
    }

    IEnumerator AnimateGroupDespawn(List<GameObject> group)
    {
        List<GameObject> activeToDespawn = new List<GameObject>();
        Dictionary<GameObject, Vector3> startPositions = new Dictionary<GameObject, Vector3>();
        Dictionary<GameObject, Vector3> startScales = new Dictionary<GameObject, Vector3>();

        foreach (GameObject npc in group)
        {
            if (npc.activeInHierarchy && !animatingNpcs.Contains(npc))
            {
                activeToDespawn.Add(npc);
                animatingNpcs.Add(npc); 
                
                Button btn = npc.GetComponent<Button>();
                if (btn != null) btn.interactable = false;

                startPositions[npc] = npc.transform.position;
                startScales[npc] = npc.transform.localScale;
            }
        }

        float duration = 0.3f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            progress = Mathf.SmoothStep(0, 1, progress); 

            foreach (GameObject npc in activeToDespawn)
            {
                Vector3 targetPos = startPositions[npc] + (Vector3.down * 1.0f);
                npc.transform.position = Vector3.Lerp(startPositions[npc], targetPos, progress);
                npc.transform.localScale = Vector3.Lerp(startScales[npc], Vector3.zero, progress);
            }
            yield return null;
        }

        foreach (GameObject npc in activeToDespawn)
        {
            npc.SetActive(false);
            npc.transform.localScale = startScales[npc]; 
            animatingNpcs.Remove(npc);
        }
    }

    public void OnNpcClicked(GameObject clickedNpc)
    {
        if (isGameOver) return;
        if (animatingNpcs.Contains(clickedNpc)) return;
        StartCoroutine(ProcessIndividualHit(clickedNpc));
    }

    IEnumerator ProcessIndividualHit(GameObject clickedNpc)
    {
        Button btn = clickedNpc.GetComponent<Button>();
        if (btn != null) btn.interactable = false;

        animatingNpcs.Add(clickedNpc);

        if (allies.Contains(clickedNpc)) 
        {
            // --- SOUND FOR CIVILIAN (OUCH) ---
            if (sfxSource != null && innocentHitClip != null) 
                sfxSource.PlayOneShot(innocentHitClip);
            
            currentPoints--;
        }
        else 
        {
            // --- SOUND FOR ENEMY (BOOM) ---
            if (sfxSource != null && enemyHitClip != null) 
                sfxSource.PlayOneShot(enemyHitClip);
            
            currentPoints++;
        }
        UpdateScoreUI();

        float animDuration = 0.4f;
        float timer = 0f;
        Vector3 originalScale = clickedNpc.transform.localScale;

        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / animDuration;
            clickedNpc.transform.Rotate(0, 0, 720 * Time.deltaTime); 
            clickedNpc.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
            yield return null;
        }

        clickedNpc.SetActive(false);
        clickedNpc.transform.localScale = originalScale;
        animatingNpcs.Remove(clickedNpc);
    }

    void EndGame()
    {
        isGameOver = true;
        // Stop music when game over
        if (musicSource != null) musicSource.Stop(); 
        
        StopAllCoroutines();
        PlayerPrefs.SetInt("FinalScore", currentPoints);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOverScene");
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Points: " + currentPoints;
    }
}