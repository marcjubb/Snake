using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ProfileButton : MonoBehaviour
{
    public int profileIndex;
    public TextMeshProUGUI profileInfoText;
    public Button profileButton; // Make sure to assign this button in the Unity Inspector

    private Profile profile;

    private void Awake()
    {
        ProfileManager.Instance.OnProfilesLoaded += UpdateProfileInfoText;
    }

    private void Start()
    {
        profile = ProfileManager.Instance.profiles[profileIndex];

        profileButton.onClick.AddListener(OnProfileButtonClick);
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ProfileManager.Instance.OnProfilesLoaded -= UpdateProfileInfoText;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "profile")
        {
            UpdateProfileInfoText();
        }
    }
    private new SceneTransition sceneTransition;

    public void OnProfileButtonClick()
    {
        ProfileManager.Instance.SetSelectedProfile(profileIndex);
        // Set the current profile in ProfileManager
        ProfileManager.Instance.profiles[profileIndex] = profile;

        // Load the game scene
        FindObjectOfType<SceneTransition>().FadeToLevel("game");
    }

 
    private void UpdateProfileInfoText()
    {
        profile = ProfileManager.Instance.profiles[profileIndex];

        // Update the profile information text
        profileInfoText.text = $"Profile {profileIndex + 1}\nAchievements: {profile.unlockedAchievements.Count} /4\nHigh Score: {profile.highScore}";
    }
}
