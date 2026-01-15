using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager menu głównego
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    
    [Header("Audio")]
    public AudioSource menuMusic;

    void Start()
    {
        // Pokaż tylko główny panel
        ShowMainPanel();
        
        // Odtwórz muzykę menu
        if (menuMusic != null && !menuMusic.isPlaying)
        {
            menuMusic.Play();
        }
    }

    public void OnPlayButton()
    {
        // Załaduj scenę gry
        SceneManager.LoadScene("SampleScene");
    }

    public void OnOptionsButton()
    {
        mainPanel?.SetActive(false);
        optionsPanel?.SetActive(true);
    }

    public void OnCreditsButton()
    {
        mainPanel?.SetActive(false);
        creditsPanel?.SetActive(true);
    }

    public void OnBackButton()
    {
        ShowMainPanel();
    }

    public void OnQuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void ShowMainPanel()
    {
        mainPanel?.SetActive(true);
        optionsPanel?.SetActive(false);
        creditsPanel?.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
