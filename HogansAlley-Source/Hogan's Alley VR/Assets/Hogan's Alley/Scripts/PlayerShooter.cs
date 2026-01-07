using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    void Update()
    {
        bool shootInput = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            shootInput = true;
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            shootInput = true;
        }

        if (shootInput)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Create a ray from the exact center of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Shoot the ray
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

            npcKilled target = hit.transform.GetComponent<npcKilled>();
            
            if (target == null && hit.transform.parent != null)
            {
                target = hit.transform.parent.GetComponent<npcKilled>();
            }

            if (target != null)
            {
                target.killed();
            }
        }
    }
}