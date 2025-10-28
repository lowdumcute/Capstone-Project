using UnityEngine;
using System.Collections.Generic;

public class ChooseRoleScreen : MonoBehaviour
{
    private Dictionary<string, string> roleSceneMap = new Dictionary<string, string>()
    {
        { "Warrior", "Warrior" },
        { "Mage", "Mage" },
        { "Archer", "Archer" }
    };

    public void ChooseRole()
    {
        CheckRole();
    }

    public void ReturnMainMenu()
    {
        SceneChangeManager.Instance.ChangeScene("MainMenu");
    }

    public void CheckRole()
    {
        string role = GameManager.Instance.dataGameManager.playerStatsUsing.characterName;

        // Nếu có scene tương ứng, load nó; nếu không thì load mặc định
        if (roleSceneMap.TryGetValue(role, out string sceneName))
        {
            SceneChangeManager.Instance.ChangeScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy scene cho role: {role}");
            SceneChangeManager.Instance.ChangeScene("MainMenu");
        }
    }
}
