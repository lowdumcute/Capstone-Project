using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public GameObject player;
    public bool Need90 = false;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Bù ngược rotation của player để icon trông đứng yên
        if(Need90)
        {
            transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(-90f, player.transform.eulerAngles.y, 0f);
        }
    }
}
