using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;
    private CardContainer cardContainer;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.DrawCards);
    }

    // Update is called once per frame
    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.DrawCards:
                Debug.Log("Drawing Cards");
                break;
            case GameState.SelectCards:
                Debug.Log("Selecting Cards");
                break;
            case GameState.PlayCards:
                Debug.Log("Playing Cards");
                break;
            case GameState.CountPlayedCards:
                Debug.Log("Counting Played Cards");
                break;
            case GameState.DestroyCards:
                Debug.Log("Destroying Cards");
                break;
        }
        OnGameStateChanged?.Invoke(newState);
    }
}

public enum GameState
{
    DrawCards,
    SelectCards,
    PlayCards,
    CountPlayedCards,
    DestroyCards
}
