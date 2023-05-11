using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    void Start()
    {
        // Check if there's an active EventSystem in the scene
        if (EventSystem.current == null)
        {
            // If not, create a new EventSystem
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
}
