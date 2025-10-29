using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// AttackCameraEffects
/// - Hoàn toàn sử dụng Cinemachine (CinemachineVirtualCamera + CinemachineBasicMultiChannelPerlin)
/// - Dùng unscaledDeltaTime để run đúng khi có slowmotion
/// - Có API: ShakeAttack(...) và ShakeUltimate(...). Bạn có thể gọi trực tiếp từ hệ thống combat.
/// </summary>
[DisallowMultipleComponent]
public class AttackCameraEffects : MonoBehaviour
{
    public PlayableDirector directorAttackZoomIn;
    public PlayableDirector directorAttackZoomOut;
    public PlayableDirector directorAttackZoomOutLong;
    public PlayableDirector directorAttackSkill;
    //public GameObject CameraMain;

    //public GameObject CameraAttackZoomIn;
    //public GameObject CameraAttackZoomOut;
    //public GameObject CameraAttackSkill;
    public void PlayAssetAssetAttackZoomOutTimeline()
    {
        directorAttackZoomOut.Play();
    }
    public void PlayAssetAssetAttackZoomOutLongTimeline()
    {
        directorAttackZoomOutLong.Play();
    }
    public void PlayAssetAttackZoomInTimeline()
    {
        directorAttackZoomIn.Play(); 
    }
    public void PlaySkillTimeline()
    {
        directorAttackSkill.Play();
    }
}
