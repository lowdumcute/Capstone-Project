using UnityEngine;
using System.Collections;

public class BossVFXController : MonoBehaviour
{
    [Header("Animator của Boss")]
    public Animator animator;

    [Header("Hiệu ứng VFX cho từng kỹ năng")]
    public ParticleSystem attackVFX;
    public ParticleSystem skill1VFX;
    public ParticleSystem skill2VFX;
    public ParticleSystem skill3VFX;
    public ParticleSystem skill4VFX;
    public ParticleSystem skill5VFX;
    public ParticleSystem skill6VFX;
    public ParticleSystem skill7VFX;

    [Header("Thời gian delay (giây) cho từng VFX")]
    public float attackDelay = 0f;
    public float skill1Delay = 0.5f;
    public float skill2Delay = 0.8f;
    public float skill3Delay = 0.5f;
    public float skill4Delay = 0.7f;
    public float skill5Delay = 0.6f;
    public float skill6Delay = 0.9f;
    public float skill7Delay = 1.2f;

    private string currentAnimName;
    private Coroutine vfxCoroutine;

    void Update()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length == 0) return;

        string animName = clipInfo[0].clip.name;

        if (animName != currentAnimName)
        {
            currentAnimName = animName;
            // Nếu có coroutine đang chạy thì dừng lại để tránh lỗi
            if (vfxCoroutine != null)
                StopCoroutine(vfxCoroutine);

            StopAllVFX();
            // Chạy coroutine delay tương ứng
            //vfxCoroutine = StartCoroutine(PlayVFXWithDelay(animName));
        }
    }

    //IEnumerator PlayVFXWithDelay(string animName)
    //{
    //    StopAllVFX();

    //    switch (animName)
    //    {
    //        case "n2017_skill_2":
    //            yield return new WaitForSeconds(attackDelay);
    //            if (attackVFX != null) attackVFX.Play();
    //            break;
    //        case "Skill1":
    //            yield return new WaitForSeconds(skill1Delay);
    //            if (skill1VFX != null) skill1VFX.Play();
    //            break;
    //        case "Skill2":
    //            yield return new WaitForSeconds(skill2Delay);
    //            if (skill2VFX != null) skill2VFX.Play();
    //            break;
    //        case "Skill3":
    //            yield return new WaitForSeconds(skill3Delay);
    //            if (skill3VFX != null) skill3VFX.Play();
    //            break;
    //        case "Skill4":
    //            yield return new WaitForSeconds(skill4Delay);
    //            if (skill4VFX != null) skill4VFX.Play();
    //            break;
    //        case "Skill5":
    //            yield return new WaitForSeconds(skill5Delay);
    //            if (skill5VFX != null) skill5VFX.Play();
    //            break;
    //        case "Skill6":
    //            yield return new WaitForSeconds(skill6Delay);
    //            if (skill6VFX != null) skill6VFX.Play();
    //            break;
    //        case "Skill7":
    //            yield return new WaitForSeconds(skill7Delay);
    //            if (skill7VFX != null) skill7VFX.Play();
    //            break;
    //    }

    //    vfxCoroutine = null;
    //}

    void StopAllVFX()
    {
        if (attackVFX != null) attackVFX.Stop();
        if (skill1VFX != null) skill1VFX.Stop();
        if (skill2VFX != null) skill2VFX.Stop();
        if (skill3VFX != null) skill3VFX.Stop();
        if (skill4VFX != null) skill4VFX.Stop();
        if (skill5VFX != null) skill5VFX.Stop();
        if (skill6VFX != null) skill6VFX.Stop();
        if (skill7VFX != null) skill7VFX.Stop();
    }
    public void PlayAttackVFX()
    {
        StopAllVFX();
        if (attackVFX != null)
            attackVFX.Play();
        vfxCoroutine = null;
    }
    public void PlaySkill3VFX()
    {
        StopAllVFX();
        if (skill3VFX != null)
            skill3VFX.Play();
        Spawnskill3lEffect();
        vfxCoroutine = null;
    }
    void Spawnskill3lEffect()
    {
        if (skill3VFX != null)
        {
            // Tính toán vị trí trước mặt player (cách 2 đơn vị)
            Vector3 spawnPosition = transform.position + transform.forward * 0.5f;

            // Bù góc xoay cho particle (vì prefab lệch -51 độ)
            Quaternion rotationOffset = Quaternion.Euler(0f, -81f, 0f);

            // Tạo instance với hướng player * bù góc
            ParticleSystem effectInstance = Instantiate(
                skill3VFX,
                spawnPosition,
                transform.rotation * rotationOffset
            );

            // Hủy sau khi hiệu ứng chạy xong
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
    }

    void no()
    {

    }
}
