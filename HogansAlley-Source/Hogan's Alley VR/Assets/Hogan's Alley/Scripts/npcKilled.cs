using UnityEngine;

public class npcKilled : MonoBehaviour
{
    public npcManager Manager;

    void Start()
    {
        
    }

    public void killed()
    {
        Debug.Log("dep");
        Manager.OnNpcClicked(transform.gameObject);
    }

    void Update()
    {
        
    }
}
