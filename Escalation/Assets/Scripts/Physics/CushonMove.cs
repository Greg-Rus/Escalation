using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CushonMove : MonoBehaviour
{
    private Rigidbody _myRigidbody;
    public float PullForce;
    public float StopVelocityThreshold;
    public float BreakModifier;
    public float BounceForce;
    public Transform BounceTarget;
    private Vector3 _bounceDirection;

    public Vector3 Vec;
    // Start is called before the first frame update
    void Awake()
    {
        _myRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _bounceDirection = BounceTarget.position - transform.position;
        _bounceDirection = _bounceDirection.normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var input = Input.GetAxisRaw("Horizontal");
        if (input.Equals(0f))
        {
            ApplyBreakingForce();
        } else if (!Mathf.Sign(_myRigidbody.velocity.x).Equals(Mathf.Sign(input)))
        {
            BreakImmediate();
        }


        Vec = PullForce * Vector3.right * input;
        _myRigidbody.AddForce(PullForce * Vector3.right * input);
    }

    private void BreakImmediate()
    {
        _myRigidbody.velocity = Vector3.zero;
    }

    private void ApplyBreakingForce()
    {
        if (_myRigidbody.velocity.sqrMagnitude > StopVelocityThreshold)
        {
            _myRigidbody.AddForce(BreakModifier * _myRigidbody.velocity * -1);
        }
        else
        {
            BreakImmediate();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
        Bounce(collision.collider);
    }

    private void Bounce(Collider collisionCollider)
    {
        var otherRigidbody = collisionCollider.attachedRigidbody;
        var velocity = otherRigidbody.velocity;
        var bouncedVelocity = new Vector3(velocity.x, velocity.y * -1, velocity.z);
        bouncedVelocity = bouncedVelocity.normalized;
        otherRigidbody.velocity = Vector3.zero;
        collisionCollider.attachedRigidbody.AddForce(_bounceDirection * BounceForce, ForceMode.Impulse);
    }
}
