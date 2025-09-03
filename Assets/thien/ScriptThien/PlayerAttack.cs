using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Animations")]
    [SerializeField] private string attack1Animation = "attack01";
    [SerializeField] private string attack2Animation = "attack02";
    [SerializeField] private string attack3Animation = "attack03";
    [SerializeField] private string attack4Animation = "attack04";

    [Header("Combo Settings")]
    [SerializeField] private float comboWindow = 0.1f;
    [SerializeField] private float minAttackDuration = 0.90f;

    [Header("Attack VFX")]
    [SerializeField] private ParticleSystem attack1VFX;
    [SerializeField] private ParticleSystem attack2VFX;
    [SerializeField] private ParticleSystem attack3VFX;
    [SerializeField] private ParticleSystem attack4VFX;

    //[Header("Attack Colliders")]
    //[SerializeField] private GameObject attack1Collider;
    //[SerializeField] private GameObject attack2Collider;
    //[SerializeField] private GameObject attack3Collider;
    //[SerializeField] private GameObject attack4Collider;

    [Header("VFX Timing")]
    [SerializeField] private float[] vfxStartDelays = new float[4] { 0.2f, 0.2f, 0.2f, 0.25f };
    [SerializeField] private float[] vfxDurations = new float[4] { 0.5f, 0.5f, 0.7f, 0.7f };

    private Animator anim;
    private int currentAttack = 0;
    private float lastClickTime = 0f;
    private bool isAttacking = false;
    private bool inputBuffered = false;
    private bool canCombo = false;
    private Player_Controller playerController;
    private Coroutine[] vfxCoroutines = new Coroutine[4];

    private void Awake()
    {
        DisableAllVFXAndColliders();
    }

    public void Start()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<Player_Controller>();
    }

    private void Update()
    {
        HandleInput();
        CheckAttackProgress();
        CheckComboTimeout();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                StartAttack();
            }
            else if (canCombo)
            {
                ContinueCombo();
            }
            else
            {
                inputBuffered = true;
            }
            lastClickTime = Time.time;
        }
    }

    private void StartAttack()
    {
        currentAttack = 1;
        StartVFXForAttack(1);
        PlayAttackAnimation(attack1Animation);

        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    private void ContinueCombo()
    {
        currentAttack++;
        if (currentAttack > 4) currentAttack = 1;

        string nextAnimation = currentAttack switch
        {
            1 => attack1Animation,
            2 => attack2Animation,
            3 => attack3Animation,
            4 => attack4Animation,
            _ => attack1Animation
        };

        StartVFXForAttack(currentAttack);
        PlayAttackAnimation(nextAnimation);
        canCombo = false;
    }

    private void StartVFXForAttack(int attackIndex)
    {
        // Hủy coroutine VFX trước đó nếu có
        if (vfxCoroutines[attackIndex - 1] != null)
        {
            StopCoroutine(vfxCoroutines[attackIndex - 1]);
        }

        // Bắt đầu coroutine mới
        vfxCoroutines[attackIndex - 1] = StartCoroutine(TriggerVFXWithDelay(attackIndex));
    }

    private IEnumerator TriggerVFXWithDelay(int attackIndex)
    {
        int idx = attackIndex - 1;
        yield return new WaitForSeconds(vfxStartDelays[idx]);

        // Kiểm tra lại trạng thái trước khi kích hoạt VFX
        if (!isAttacking || currentAttack != attackIndex)
            yield break;

        // Tắt tất cả VFX và collider trước khi bật cái mới
        DisableAllVFXAndColliders();

        switch (attackIndex)
        {
            case 1:
                attack1VFX.Play();
               // attack1Collider.SetActive(true);
                break;
            case 2:
                attack2VFX.Play();
               // attack2Collider.SetActive(true);
                break;
            case 3:
                attack3VFX.Play();
               // attack3Collider.SetActive(true);
                break;
            case 4:
                attack4VFX.Play();
                //attack4Collider.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(vfxDurations[idx]);

        //// Chỉ tắt collider nếu vẫn đang ở cùng đòn tấn công
        //if (currentAttack == attackIndex)
        //{
        //    switch (attackIndex)
        //    {
        //        case 1: attack1Collider.SetActive(false); break;
        //        case 2: attack2Collider.SetActive(false); break;
        //        case 3: attack3Collider.SetActive(false); break;
        //        case 4: attack4Collider.SetActive(false); break;
        //    }
        //}
    }

    private void DisableAllVFXAndColliders()
    {
        attack1VFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        attack2VFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        attack3VFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        attack4VFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        //attack1Collider.SetActive(false);
        //attack2Collider.SetActive(false);
        //attack3Collider.SetActive(false);
        //attack4Collider.SetActive(false);
    }

    private void PlayAttackAnimation(string animationName)
    {
        anim.Play(animationName, 0, 0f);
        isAttacking = true;
        canCombo = false;
    }

    private void CheckAttackProgress()
    {
        if (isAttacking)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime >= minAttackDuration)
            {
                canCombo = true;

                if (inputBuffered)
                {
                    inputBuffered = false;
                    ContinueCombo();
                }
            }

            if (stateInfo.normalizedTime >= 1f)
            {
                ReturnToIdle();
            }
        }
    }

    private void ReturnToIdle()
    {
        anim.SetBool("isidle", true);
        currentAttack = 0;
        isAttacking = false;
        inputBuffered = false;
        canCombo = false;
        DisableAllVFXAndColliders();

        // Hủy tất cả coroutine VFX khi kết thúc combo
        for (int i = 0; i < 4; i++)
        {
            if (vfxCoroutines[i] != null)
            {
                StopCoroutine(vfxCoroutines[i]);
                vfxCoroutines[i] = null;
            }
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private void CheckComboTimeout()
    {
        if (Time.time - lastClickTime > comboWindow && currentAttack > 0)
        {
            ReturnToIdle();
        }
    }
}