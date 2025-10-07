using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CharTimeLine : MonoBehaviour
{
    public PlayableDirector Director;

    public TimelineAsset clipAttack1;
    public TimelineAsset clipAttack2;
    
    public void Attack1()
    {
        Director.playableAsset = clipAttack1;
        Director.Play(); 
    }
    public void Attack2()
    {
        Director.playableAsset = clipAttack2;
        Director.Play(); 
    }
}
