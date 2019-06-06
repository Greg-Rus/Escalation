using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BouncingController : MonoBehaviour
{
    private Sequence _jumpTween; 
    // Start is called before the first frame update

    public void JumpTo(Vector3 jumpEnd, float jumpHeight, float duration)
    {
        _jumpTween?.Complete();
        _jumpTween?.Kill();
        _jumpTween = transform.DOJump(jumpEnd, jumpHeight, 1, duration).Append(transform.DOPunchScale(new Vector3(0.5f, -0.5f, 0.5f),0.5f));
    }
}
