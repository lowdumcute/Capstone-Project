using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Testing : MonoBehaviour
{
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }
    }

    public void Save()
    {

        GameManager.Instance.SaveProgress();


    }
}
