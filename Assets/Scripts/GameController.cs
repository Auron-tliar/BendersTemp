using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public List<PlayerController> PlayerControllers;

    [HideInInspector]
    public List<AIAgent> AiAgents;

    public AudioSource AudioPlayer;
    public List<AudioClip> BackgroundMusic;

    public Text Minutes;
    public Text Seconds;

    public Transform PlayersContainer;
    public List<Transform> BenderIconPanels;
    public GameObject AbilityPanels;
    public List<Transform> SpawnPoints;

    public GameObject HumanMousePlayerPrefab;
    public GameObject HumanKeyboardPlayerPrefab;
    public GameObject AIPlayerPrefab;

    public GameObject AIAgentPrefab;

    [Header("Bender Prefabs")]
    public GameObject AirBenderPrefab;
    public GameObject EarthBenderPrefab;
    public GameObject FireBenderPrefab;
    public GameObject WaterBenderPrefab;

    public MLAgents.Brain brain;

    private int _currentTrack = -1;

    private void Start()
    {
        PlayNextTrack();
        for (int i = 0; i < MatchSettings.Players.Count; i++)
        {
            switch (MatchSettings.PlayerTypes[i])
            {
                case PlayerController.PlayerTypes.HumanMouse:
                    PlayerControllers.Add(Instantiate(HumanMousePlayerPrefab, 
                        PlayersContainer).GetComponent<PlayerController>());
                    ((HumanControllerMouse)PlayerControllers[i]).Panels = AbilityPanels;
                    break;
                case PlayerController.PlayerTypes.HumanKeyBoard:
                    PlayerControllers.Add(Instantiate(HumanKeyboardPlayerPrefab,
                        PlayersContainer).GetComponent<PlayerController>());
                    ((HumanControllerKeyBoard)PlayerControllers[i]).Panels = AbilityPanels;
                    break;
                case PlayerController.PlayerTypes.AI:
                    PlayerControllers.Add(Instantiate(AIPlayerPrefab,
                        PlayersContainer).GetComponent<PlayerController>());
                    break;
                default:
                    break;
            }
            PlayerControllers[i].PlayerColor = MatchSettings.PlayerColors[i];
            PlayerControllers[i].UIIconContainer = BenderIconPanels[i];
            
            for (int j = 0; j < MatchSettings.Players[i].Count; j++)
            {
                GameObject temp;
                switch (MatchSettings.Players[i][j])
                {
                    case Bender.BenderTypes.Air:
                        temp = AirBenderPrefab;
                        break;
                    case Bender.BenderTypes.Earth:
                        temp = EarthBenderPrefab;
                        break;
                    case Bender.BenderTypes.Fire:
                        temp = FireBenderPrefab;
                        break;
                    case Bender.BenderTypes.Water:
                        temp = WaterBenderPrefab;
                        break;
                    default:
                        temp = null;
                        break;
                }


                if (MatchSettings.PlayerTypes[i] == PlayerController.PlayerTypes.AI)
                {
                    //temp.SetActive(false);

                    AIAgentPrefab.SetActive(false);

                    GameObject aiAgent = Instantiate(AIAgentPrefab, SpawnPoints[i].GetChild(j).position, SpawnPoints[i].GetChild(j).rotation, PlayerControllers[i].transform);

                    aiAgent.GetComponent<AIAgent>().brain = brain;
                    //aiAgent.GetComponent<AIAgent>().randomActionProbabiliy = 0.1f;
                    aiAgent.GetComponent<AIAgent>().noActionInterval = 20;

                    GameObject bender = Instantiate(temp, SpawnPoints[i].GetChild(j).position, Quaternion.identity, aiAgent.transform);
                    bender.GetComponent<Bender>().Owner = PlayerControllers[i];

                    aiAgent.GetComponent<AIAgent>().bender = bender.GetComponent<Bender>();

                    //bender.SetActive(true);

                    aiAgent.SetActive(true);

                }
                else
                {
                    Instantiate(temp, SpawnPoints[i].GetChild(j).position,
                        SpawnPoints[i].GetChild(j).rotation, PlayerControllers[i].transform);
                }
            }
            
        }
    }

    private void Update()
    {
        Minutes.text = ((int)Time.time / 60).ToString();
        Seconds.text = ((int)Time.time % 60).ToString();

        if (!AudioPlayer.isPlaying)
        {
            PlayNextTrack();
        }

        int numAi = FindObjectsOfType<AIAgent>().Count((AIAgent ai) => ai.GetComponentInChildren<Bender>() != null);
        int numPlayers = FindObjectsOfType<HumanController>().Count((HumanController player) => player.GetComponentInChildren<Bender>() != null);

        Debug.Log(numAi);
        Debug.Log(numPlayers);

        if (numPlayers == 0)
        {
            MatchSettings.WinnerTeam = 1;
            SceneManager.LoadScene("Menu");
        }
        else if(numAi == 0)
        {
            MatchSettings.WinnerTeam = 0;
            SceneManager.LoadScene("Menu");
        }

    }

    private void PlayNextTrack()
    {
        _currentTrack++;
        _currentTrack %= BackgroundMusic.Count;
        AudioPlayer.clip = BackgroundMusic[_currentTrack];
        AudioPlayer.Play();
    }
}
