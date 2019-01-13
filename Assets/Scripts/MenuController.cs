using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Image Background;
    public GameObject MainMenu;
    public GameObject SelectionMenu;

    public AudioSource AudioPlayer;

    public List<MenuPlayerContainer> PlayerContainers;

    public string MatchSceneName;

    private bool _isDragging = false;
    private int _numHumans = 0;

    public bool IsDragging
    {
        get
        {
            return _isDragging;
        }

        set
        {
            _isDragging = value;
            foreach (MenuPlayerContainer c in PlayerContainers)
            {
                c.IsCatching = _isDragging;
            }
        }
    }

    private void Start()
    {
        Background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        if(MatchSettings.WinnerTeam.HasValue && MatchSettings.WinnerTeam.Value == 0)
        {
            GameObject title = GameObject.Find("Title");
            title.GetComponent<Text>().text = "You won!";
        } else if(MatchSettings.WinnerTeam.HasValue && MatchSettings.WinnerTeam.Value == 1)
        {
            GameObject title = GameObject.Find("Title");
            title.GetComponent<Text>().text = "Game Over!";
        }
    }

    public void NewMatch()
    {
        MainMenu.SetActive(false);
        SelectionMenu.SetActive(true);
        Background.color = Color.gray;
    }

    public void Quit()
    {
        Debug.Log("Exiting...");
        Application.Quit();
    }

    public void StartMatch()
    {
        Debug.Log("Starting match...");
        MatchSettings.Players = new List<List<Bender.BenderTypes>>();
        MatchSettings.PlayerColors = new List<Color>();
        MatchSettings.PlayerTypes = new List<PlayerController.PlayerTypes>();
        for (int i = 0; i < PlayerContainers.Count; i++)
        {
            MatchSettings.Players.Add(PlayerContainers[i].GetBendersList());
            MatchSettings.PlayerColors.Add(PlayerContainers[i].PlayerColor);
            if (PlayerContainers[i].PlayerType.value == 0)
            {
                _numHumans++;
                if (_numHumans == 1)
                {
                    MatchSettings.PlayerTypes.Add(PlayerController.PlayerTypes.HumanMouse);
                }
                else if (_numHumans == 2)
                {
                    MatchSettings.PlayerTypes.Add(PlayerController.PlayerTypes.HumanKeyBoard);
                }
                else
                {
                    throw new System.Exception("More than 2 human players!");
                }
            }
            else
            {
                MatchSettings.PlayerTypes.Add(PlayerController.PlayerTypes.AI);
            }
        }
        AudioPlayer.Stop();
        SceneManager.LoadScene(MatchSceneName);
    }

    public void Back()
    {
        SelectionMenu.SetActive(false);
        MainMenu.SetActive(true);
        Background.color = Color.white;
    }
}
