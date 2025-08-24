using UnityEngine;

namespace Assets.ImagyVFX.Scripts.EffectsSequence
{
    internal sealed class SequencePart : MonoBehaviour
    {
        public GameObject EffectPrefab;
        public float LifeTime;
        public float CallNextDelay;

        public void Run(Transform transformParent)
        {
            var go = Instantiate(EffectPrefab);
            go.SetActive(true);
            go.transform.parent = transformParent;
            go.transform.position = transformParent.position + EffectPrefab.transform.position;
            Destroy(go, LifeTime);
        }
    }
}