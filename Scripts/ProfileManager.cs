using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }
    public Profile[] profiles;

    public event Action OnProfilesLoaded;


   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "profile")
        {
            LoadProfiles(); // Load the profiles when the profile scene is loaded
        }
    }

    public void ResetProfileData(int profileIndex)
    {
        Profile profileToReset = profiles[profileIndex];
        profileToReset.highScore = 0;
        profileToReset.unlockedAchievements.Clear();
        SaveProfiles(); // Assuming you have a function that saves the profiles to disk
    }


    public void SaveProfiles()
    {
        for (int i = 0; i < profiles.Length; i++)
        {
            PlayerPrefs.SetString("Profile_" + i, JsonUtility.ToJson(profiles[i]));
        }
    }

    public void LoadProfiles()
    {
        for (int i = 0; i < profiles.Length; i++)
        {
            if (PlayerPrefs.HasKey("Profile_" + i))
            {
                profiles[i] = JsonUtility.FromJson<Profile>(PlayerPrefs.GetString("Profile_" + i));
            }
            else
            {
                profiles[i] = new Profile();
            }
        }

        // Raise the OnProfilesLoaded event
        OnProfilesLoaded?.Invoke();
    }

    public int SelectedProfileIndex { get; private set; }

    public void SetSelectedProfile(int index)
    {
        if (index >= 0 && index < profiles.Length)
        {
            SelectedProfileIndex = index;
        }
        else
        {
            // Handle invalid profile index
        }
    }

    public void UpdateHighScore(int newHighScore)
    {
        if (newHighScore > profiles[SelectedProfileIndex].highScore)
        {
            profiles[SelectedProfileIndex].highScore = newHighScore;
            SaveProfiles(); // Save the updated profiles to PlayerPrefs
        }
    }

    public void UpdateAchievements(List<Achievement> achievements)
    {
        Profile selectedProfile = profiles[SelectedProfileIndex];
        selectedProfile.unlockedAchievements.Clear();

        foreach (Achievement achievement in achievements)
        {
            if (achievement.isUnlocked)
            {
                selectedProfile.unlockedAchievements.Add(achievement.id);
            }
        }

        SaveProfiles(); // Save the updated profiles to PlayerPrefs
    }

    public void BackToMenu()
    {
        FindObjectOfType<SceneTransition>().FadeToLevel("menu"); // Replace "Menu" with the name of your menu scene
    }

}
