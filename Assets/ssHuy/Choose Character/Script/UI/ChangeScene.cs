using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame : MonoBehaviour
{
    public void Change(string NameScene)
    {
        GameManager.Instance.dataGameManager.Position = new Vector3(548, 11, 375);
        //SceneChangeManager.Instance.LoadScene(NameScene);
    }
}
