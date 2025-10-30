using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ChangeSence(string NameScene)
    {
        SceneChangeManager.Instance.ChangeScene(NameScene);
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
