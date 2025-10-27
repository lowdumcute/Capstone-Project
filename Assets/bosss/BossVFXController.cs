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

   

    void StopAllVFX()
    {
        if (attackVFX != null) attackVFX.Stop();
        if (skill1VFX != null) skill1VFX.Stop();
        if (skill2VFX != null) skill2VFX.Stop();
        if (skill3VFX != null) skill3VFX.Stop();
       
    }
    public void PlayAttackVFX()
    {
        StopAllVFX();
        if (attackVFX != null)
            attackVFX.Play();
        vfxCoroutine = null;
    }
    public void PlayAskill1VFX()
    {
        StopAllVFX();
        if (skill1VFX != null)
            skill1VFX.Play();
        Spawnskill1lEffect();
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
    void Spawnskill1lEffect()
    {
        if (skill1VFX != null)
        {
            // Tính toán vị trí trước mặt player (cách 2 đơn vị)
            Vector3 spawnPosition = transform.position + transform.forward * 0f;

            // Bù góc xoay cho particle (vì prefab lệch -51 độ)
            Quaternion rotationOffset = Quaternion.Euler(0f, 0f, 0f);

            // Tạo instance với hướng player * bù góc
            ParticleSystem effectInstance = Instantiate(
                skill1VFX,
                spawnPosition,
                transform.rotation * rotationOffset
            );

            // Hủy sau khi hiệu ứng chạy xong
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
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
