using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public List<PlayerController> PlayerControllers;

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

    [Header("Bender Prefabs")]
    public GameObject AirBenderPrefab;
    public GameObject EarthBenderPrefab;
    public GameObject FireBenderPrefab;
    public GameObject WaterBenderPrefab;

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
                Instantiate(temp, SpawnPoints[i].GetChild(j).position,
                    SpawnPoints[i].GetChild(j).rotation, PlayerControllers[i].transform);
            }
            PlayerControllers[i].PlayerColor = MatchSettings.PlayerColors[i];
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
    }

    private void PlayNextTrack()
    {
        _currentTrack++;
        _currentTrack %= BackgroundMusic.Count;
        AudioPlayer.clip = BackgroundMusic[_currentTrack];
        AudioPlayer.Play();
    }
}
