using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public Player[] players;
    public int activePlayerIndex = 0;
    public int turn = 1;

    public TextMeshProUGUI turnDisplay;
    public TextMeshProUGUI bannerText;
    public CanvasGroup turnBanner;

    public float fadeSpeed = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        players[activePlayerIndex].isPlayerTurn = true;
        turnBanner.alpha = 1;
        bannerText.text = $"{players[activePlayerIndex].playerName}'s Turn";
        turnDisplay.text = $"Turn: {turn}";
    }

    // Update is called once per frame
    void Update()
    {
        if (turnBanner.alpha > 0)
        {
            turnBanner.alpha -= fadeSpeed * Time.deltaTime;
        }
    }

    public void EndTurn()
    {
        players[activePlayerIndex].ResetUnits();
        players[activePlayerIndex].isPlayerTurn = false;    //Deactivate previous player's Units
        activePlayerIndex = (activePlayerIndex + 1) % players.Length;   //Increase player index mod the length (will equal 0 if at the last player)
        players[activePlayerIndex].isPlayerTurn = true;     //Activate next player's Units

        //If at the last player in turn order, increment the turn counter
        if (activePlayerIndex == 0)
        {
            turn++;
            turnDisplay.text = $"Turn: {turn}";
        }

        bannerText.text = $"{players[activePlayerIndex].playerName}'s Turn";
        turnBanner.alpha = 1;
    }
}
