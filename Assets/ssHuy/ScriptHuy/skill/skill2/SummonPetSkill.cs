using UnityEngine;

public class SummonPetSkill : SkillBehaviour
{
    public GameObject petPrefab;

    public override void UseSkill(Transform firePoint, GameObject target)
    {
        base.UseSkill(firePoint, target);

        if (petPrefab == null || firePoint == null) return;

        GameObject pet = Instantiate(petPrefab, firePoint.position, Quaternion.identity);

        PetController petController = pet.GetComponent<PetController>();
        if (petController != null)
        {
            petController.player = GameObject.FindGameObjectWithTag("Player")?.transform;
            petController.lifetime = 10f; // Tồn tại 10 giây
        }
    }
}