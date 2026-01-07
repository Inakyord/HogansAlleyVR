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

    private List<GameObject> allies = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();
    private int currentPoints = 0;
    private float timeLeft = 40f;
    private bool isGameOver = false;
    private bool npcClickedInThisTurn = false;

    private bool isProcessingHit = false; 

    void Start()
    {
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

        if (Camera.main != null)
        {
            foreach (GameObject npc in npcs)
            {
                if (npc.activeInHierarchy)
                    npc.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }

    IEnumerator MainGameCycle()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(Random.Range(0f, 3f));

            if (isGameOver) break;

            yield return StartCoroutine(SpawnGroupRoutine());
        }
    }

    IEnumerator SpawnGroupRoutine()
    {
        npcClickedInThisTurn = false;

        List<GameObject> availableEnemies = enemies.Where(e => !e.activeInHierarchy).ToList();
        List<GameObject> availableAllies = allies.Where(a => !a.activeInHierarchy).ToList();
        List<GameObject> availablePlaces = places.OrderBy(x => Random.value).Take(3).ToList();

        if (availableEnemies.Count >= 2 && availableAllies.Count >= 1 && availablePlaces.Count >= 3)
        {
            List<GameObject> selectedEnemies = availableEnemies.OrderBy(x => Random.value).Take(2).ToList();
            GameObject selectedAlly = availableAllies[Random.Range(0, availableAllies.Count)];

            List<GameObject> group = new List<GameObject>(selectedEnemies) { selectedAlly };

            for (int i = 0; i < 3; i++)
            {
                group[i].transform.position = availablePlaces[i].transform.position;
                Button btn = group[i].GetComponent<Button>();
                if (btn != null) btn.interactable = true;
                group[i].SetActive(true);
            }

            float elapsed = 0f;
            while (elapsed < 3f && !npcClickedInThisTurn)
            {
                elapsed += Time.deltaTime;
                yield return null; 
            }

            if (npcClickedInThisTurn)
            {
                yield return new WaitUntil(() => isProcessingHit == false);
            }
            else
            {
                foreach (GameObject npc in group) npc.SetActive(false);
            }
        }
    }

    public void OnNpcClicked(GameObject clickedNpc)
    {
        if (npcClickedInThisTurn || isGameOver) return;
        StartCoroutine(ProcessGlobalHit(clickedNpc));
    }

    IEnumerator ProcessGlobalHit(GameObject clickedNpc)
    {
        npcClickedInThisTurn = true;
        isProcessingHit = true;

        clickedNpc.SetActive(false);

        foreach (GameObject npc in npcs)
        {
            if (npc.activeInHierarchy)
            {
                Button btn = npc.GetComponent<Button>();
                if (btn != null) btn.interactable = false;
            }
        }

        if (allies.Contains(clickedNpc)) currentPoints--;
        else currentPoints++;
        UpdateScoreUI();

        yield return new WaitForSeconds(1f);

        foreach (GameObject npc in npcs) npc.SetActive(false);

        isProcessingHit = false;
    }

    void EndGame()
    {
        isGameOver = true;
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