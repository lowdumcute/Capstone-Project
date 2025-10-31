using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void newGame()
    {
        SaveLoadManager.Instance.ResetSaveName();
        SceneChangeManager.Instance.ChangeScene("ChooseCharacter");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenLoadPanel()
    {
        SaveLoadManager.Instance.OpenLoadPanel();
    }
}
