using TMPro;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    //[Header("Current Level Settings")]
    //public string thisLevelName;
    //public TextMeshProUGUI levelLabel;
    [Header("Next Level Settings")]
    public string nextLevelName;
    public int nextLevelValue;

    

    //private void Start()
    //{
    //    if (levelLabel != null)
    //        levelLabel.text = thisLevelName;

    //}
    public void LoadNextLevel()
    {
        PlayerPrefs.SetInt("levelReached", nextLevelValue);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
        Time.timeScale = 1;
    }
}
