using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private List<GameObject> tutorialMenus = new List<GameObject>();
    private int currentMenuIndex = 0;
    [SerializeField] private GameObject nextPanelButton;
    [SerializeField] private GameObject previousPanelButton;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene" && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            FindFirstObjectByType<Typer>().enabled = true;
            Time.timeScale = 1f;
        }
        else
        {
            pauseMenu.SetActive(true);
            FindFirstObjectByType<Typer>().enabled = false;
            Time.timeScale = 0f;
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public void NextMenu()
    {
        if (currentMenuIndex < tutorialMenus.Count - 1)
        {
            tutorialMenus[currentMenuIndex].SetActive(false);
            currentMenuIndex++;
            tutorialMenus[currentMenuIndex].SetActive(true);

            if (currentMenuIndex == tutorialMenus.Count - 1)
            {
                nextPanelButton.SetActive(false);
            }

            if (currentMenuIndex > 0)
            {
                previousPanelButton.SetActive(true);
            }
        }
    }

    public void PreviousMenu()
    {
        if (currentMenuIndex > 0)
        {
            tutorialMenus[currentMenuIndex].SetActive(false);
            currentMenuIndex--;
            tutorialMenus[currentMenuIndex].SetActive(true);

            if (currentMenuIndex < tutorialMenus.Count - 1)
            {
                nextPanelButton.SetActive(true);
            }

            if (currentMenuIndex == 0)
            {
                previousPanelButton.SetActive(false);
            }
        }
    }
}