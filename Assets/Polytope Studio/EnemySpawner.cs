using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySlot
    {
        public GameObject prefab;
        [HideInInspector] public GameObject instance;
    }

    public List<EnemySlot> enemies = new List<EnemySlot>();
    public float respawnTime = 5f;

    private Dictionary<Enemy, float> respawnTimers = new Dictionary<Enemy, float>();

    void Start()
    {
        //foreach (var slot in enemies)
        //{
        //    // Spawn ban đầu
        //    slot.instance = Instantiate(slot.prefab, transform.position, Quaternion.identity);
        //    var enemyScript = slot.instance.GetComponent<Enemy>();
        //    enemyScript.spawner = this;
        //}
    }

    void Update()
    {
        var keys = new List<Enemy>(respawnTimers.Keys);
        foreach (var enemy in keys)
        {
            respawnTimers[enemy] += Time.deltaTime;
            if (respawnTimers[enemy] >= respawnTime)
            {
                enemy.Respawn(); // bật lại enemy
                respawnTimers.Remove(enemy);
            }
        }
    }

    public void NotifyEnemyDeath(Enemy enemy)
    {
        // tắt enemy, cho vào danh sách chờ hồi sinh
        enemy.gameObject.SetActive(false);
        respawnTimers[enemy] = 0f;
    }
}
