using UnityEngine;

public interface ICharacterController
{
    void SetMovementEnabled(bool enabled);
    Transform transform { get; }
}
