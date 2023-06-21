using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;

    public void SetPlayerScore(string playerName, int score)
    {
        nameText.text = playerName;
        scoreText.text = score.ToString();
    }
}
