using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ChangeSence(string NameScene)
    {
        SceneChangeManager.Instance.ChangeScene(NameScene);
    }
}
