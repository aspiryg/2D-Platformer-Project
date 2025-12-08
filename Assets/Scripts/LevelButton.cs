using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int level;
    private void Start()
    {
        Button btn = GetComponent<Button>();
        if (PlayerPrefs.GetInt("levelReached") < level)
        {
            btn.interactable = false;
        }
    }
}
