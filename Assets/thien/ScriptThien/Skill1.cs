using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill1 : MonoBehaviour
{
    // nut E
    [Header("Animation Settings")]
    public Animator playerAnimator;
    public string skillAnimationName = "skill1";
    [Tooltip("Tốc độ animation sẽ giảm xuống 0.2 sau khi hết thời gian đóng băng")]
    public float slowedDownSpeed = 0.01f;

    [Header("Effect Settings")]
    public ParticleSystem mathuatVFX1;
    public ParticleSystem quanlimathuat2;
    public ParticleSystem tuNangLuong;
    public ParticleSystem chemkill1;

    public GameObject chay;
    [Tooltip("Thời gian playback của từ năng lượng (giây)")]
    public float tuNangLuongDuration = 9f;
    public float delayBeforeAnimation = 0.8f;

    [Header("Sword Material Settings")]
    public Material sword12Material;
    public Material sword13Material;
    public Renderer swordRenderer;

    [Header("Player Control")]
    public MonoBehaviour playerController;
    public KeyCode[] movementKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D }; // Các phím di chuyển cần khóa

    [Header("UI Cooldown Settings")]
    public Image cooldownImage;            // Gắn Image UI vào đây
    public float cooldownDuration = 15f;   // Thời gian hồi chiêu
    public TextMeshProUGUI cooldownText;



    private bool isSkillReady = true;
    private float originalAnimationSpeed;
    private bool isTuNangLuongActive = false;
    private bool isMovementLocked = false;

    void Start()
    {
        chay.SetActive(false);
        InitializeParticleSystems();

        if (playerAnimator != null)
        {
            originalAnimationSpeed = playerAnimator.speed;
        }
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // ban đầu chưa cooldown
        }
        if (cooldownText != null)
            cooldownText.text = "";
    }

    void InitializeParticleSystems()
    {
        if (mathuatVFX1 != null) mathuatVFX1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (quanlimathuat2 != null) quanlimathuat2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (tuNangLuong != null) tuNangLuong.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (chemkill1 != null) chemkill1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isSkillReady)
        {
            StartSkillSequence();
            StartCoroutine(CooldownRoutine()); // Bắt đầu hồi chiêu khi dùng skill

        }

        // Nếu đang trong trạng thái khóa di chuyển
        if (isMovementLocked)
        {
            foreach (KeyCode key in movementKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    // Ngăn không cho xử lý phím di chuyển
                    return;
                }
            }
        }
    }

    void StartSkillSequence()
    {
        if (playerController != null)
        {
            playerController.enabled = false; // Vô hiệu hóa Player_Controller khi bắt đầu tấn công
        }
        isSkillReady = false;
     

        if (mathuatVFX1 != null) mathuatVFX1.Play();
        else Debug.LogWarning("Particle System not assigned for Skill1");

        Invoke("PlaySkillAnimation", delayBeforeAnimation);
    }

    void PlaySkillAnimation()
    {
        playerAnimator.Play(skillAnimationName, 0, 0f);
        if (quanlimathuat2 != null) quanlimathuat2.Play();

        Invoke("ReduceParticleEffect", 3.1f);
        Invoke("ActivateTuNangLuong", 1.2f);
        Invoke("SpawnChemkillEffect", 3.6f); // goi chem sau 2f

        if (playerAnimator == null)
        {
            Debug.LogWarning("Animator not assigned for Skill1");
            ResetSkill();
            return;
        }
    }

 
    void ReduceParticleEffect()
    {
        if (quanlimathuat2 != null) quanlimathuat2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void ActivateTuNangLuong()
    {
        if (tuNangLuong != null)
        {
            tuNangLuong.Play();
            isTuNangLuongActive = true;
            Invoke("ActivateSwordEffects", 0.99f);
        }

        ContinueAnimation();
    }

    void ActivateSwordEffects()
    {
        if (swordRenderer != null && sword13Material != null)
        {
            swordRenderer.material = sword13Material;
            chay.SetActive(true);
        }
    }

    void ContinueAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.speed = slowedDownSpeed;
            Invoke("RestoreAnimationSpeed", 1.9f);
        }

        if (isTuNangLuongActive)
        {
            Invoke("StopTuNangLuong", 2.7f);
        }

        Invoke("ResetSkill", GetAnimationLength());
    }

    void StopTuNangLuong()
    {
        if (tuNangLuong != null) tuNangLuong.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        isTuNangLuongActive = false;
    }

    void RestoreAnimationSpeed()
    {
        if (playerAnimator != null) playerAnimator.speed = originalAnimationSpeed;
    }

    float GetAnimationLength()
    {
        if (playerAnimator != null)
        {
            AnimationClip[] clips = playerAnimator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == skillAnimationName)
                {
                    return clip.length;
                }
            }
        }
        return 1f;
    }

    void ResetSkill()
    {
        InitializeParticleSystems();
       
            chay.SetActive(false);

        if (swordRenderer != null && sword12Material != null)
        {
            swordRenderer.material = sword12Material;
        }
        if (playerAnimator != null) playerAnimator.speed = originalAnimationSpeed;

        if (playerController != null)
        {
            playerController.enabled = true; // Kích hoạt lại Player_Controller khi kết thúc tấn công
        }

      //  isSkillReady = true;
        isTuNangLuongActive = false;
    }
    void SpawnChemkillEffect()
    {
        if (chemkill1 != null)
        {
            // Tính toán vị trí trước mặt player (cách 2 đơn vị)
            Vector3 spawnPosition = transform.position + transform.forward * 2f;

            // Tạo instance của particle system
            ParticleSystem effectInstance = Instantiate(chemkill1, spawnPosition, transform.rotation);

            // Kích hoạt và chạy hiệu ứng
            effectInstance.gameObject.SetActive(true);
            effectInstance.Play();

            // Hủy sau khi hoàn thành
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
    }
    public void OnSkillAnimationEnd()
    {
        // Logic bổ sung khi kết thúc animation
    }
    // hoi chieu
    IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;

        if (cooldownImage != null)
            cooldownImage.fillAmount = 1f;

        if (cooldownText != null)
            cooldownText.text = cooldownDuration.ToString("0");

        while (elapsed < cooldownDuration)
        {
            elapsed += Time.deltaTime;

            if (cooldownImage != null)
                cooldownImage.fillAmount = 1f - (elapsed / cooldownDuration);

            if (cooldownText != null)
            {
                float remaining = Mathf.Ceil(cooldownDuration - elapsed);
                cooldownText.text = remaining.ToString("0");
            }

            yield return null;
        }

        if (cooldownImage != null)
            cooldownImage.fillAmount = 0f;

        if (cooldownText != null)
            cooldownText.text = " ";

        isSkillReady = true;
    }
}