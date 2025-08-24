using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f; // thời gian tồn tại của VFX

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
