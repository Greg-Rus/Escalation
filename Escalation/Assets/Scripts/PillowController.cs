using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public enum PillowBehaviorType
{
    FixedJumpRight,
    ProportionalJumpBothSides,
    ProportionalJumpRight
}

public enum JumpType
{
    RandomHeight,
    BoostableHeight
}

[RequireComponent(typeof(Rigidbody))]
public class PillowController : MonoBehaviour
{
    private Rigidbody _myRigidbody;
    public Transform LeftEdge;
    public Transform RightEdge;
    public BoxCollider PillowCollider;
    private Vector3 _maxLeft;
    private Vector3 _maxRight;
    public Transform Model;

    public float JumpHeightMin;
    public float JumpHeightMax;
    public float JumpDuration;
    public float BoostBounceGraceTime;
    private float _boostBounceTimeLeft;
    private Tween _pulseTween;
    public float MinJump;
    public float MaxJump;
    public float FixedJumpDistance;
    public PillowBehaviorType PillowBehaviorType;
    public JumpType JumpType;


    public float MovementSpeed;
    // Start is called before the first frame update
    void Awake()
    {
        _myRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _maxLeft = LeftEdge.position + Vector3.right * (PillowCollider.size.x * 0.5f);
        _maxRight = RightEdge.position + Vector3.left * (PillowCollider.size.x * 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        var moveDirection = Input.GetAxisRaw("Horizontal");
        
        transform.Translate(Vector3.right * moveDirection * MovementSpeed * Time.smoothDeltaTime);
        if (transform.position.x > _maxRight.x) transform.position = _maxRight;
        if (transform.position.x < _maxLeft.x) transform.position = _maxLeft;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _boostBounceTimeLeft = BoostBounceGraceTime;
            AnimateBoostPulse();
        }

        if (_boostBounceTimeLeft > 0)
        {
            _boostBounceTimeLeft -= Time.deltaTime;
        }
    }

    private void AnimateBoostPulse()
    {
        _pulseTween?.Complete();
        _pulseTween?.Kill();
        _pulseTween = Model.DOPunchScale(new Vector3(-0.2f,2f, -0.2f), BoostBounceGraceTime, 20, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        var jumpHeight = CalculateJumpHeight();
        var jumpDistance = CalculateJumpDistance(other);
        ExecuteJump(other, jumpDistance, jumpHeight);
        AnimatePillowBounceEffect();
    }

    private float CalculateJumpHeight()
    {
        switch (JumpType)
        {
            case JumpType.RandomHeight:
                return Random.Range(JumpHeightMin, JumpHeightMax);
            case JumpType.BoostableHeight:
                if (_boostBounceTimeLeft > 0) return JumpHeightMax;
                else return JumpHeightMin;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void ExecuteJump(Collider other, float jumpDistance, float jumpHeight)
    {
        var controller = other.gameObject.GetComponent<BouncingController>();
        if (controller == null)
        {
            Debug.LogError("Pillow Collided with " + other.name + " and expected to bounce it");
        }
        else
        {
            controller.JumpTo(transform.position + Vector3.right * jumpDistance, jumpHeight, JumpDuration);
        }
    }

    private void AnimatePillowBounceEffect()
    {
        _pulseTween?.Complete();
        _pulseTween?.Kill();

        _pulseTween = Model.DOPunchScale(new Vector3(0.5f, -0.5f, 0.5f), 0.5f);
    }

    private float CalculateJumpDistance(Collider other)
    {
        switch (PillowBehaviorType)
        {
            case PillowBehaviorType.FixedJumpRight:
                return FixedRightJump();
            case PillowBehaviorType.ProportionalJumpBothSides:
                return CalculateProportionalJump(other);
            case PillowBehaviorType.ProportionalJumpRight:
                return CalculateProportionalJumpRight(other);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private float CalculateProportionalJump(Collider other)
    {
        var dirToObject = other.transform.position - transform.position;
        var jumpMagnitude = Mathf.InverseLerp(0f, PillowCollider.size.x * 0.5f, Mathf.Abs(dirToObject.x));
        var jumpDistance = Mathf.Lerp(MinJump, MaxJump, jumpMagnitude) * Mathf.Sign(dirToObject.x);
        return jumpDistance;
    }

    private float FixedRightJump()
    {
        return FixedJumpDistance;
    }

    private float CalculateProportionalJumpRight(Collider other)
    {
        var dirToObject = other.transform.position - transform.position;
        var jumpMagnitude = Mathf.InverseLerp(PillowCollider.size.x * -0.5f, PillowCollider.size.x * 0.5f, dirToObject.x);
        var jumpDistance = Mathf.Lerp(MinJump, MaxJump, jumpMagnitude);
        return jumpDistance;
    }
}
