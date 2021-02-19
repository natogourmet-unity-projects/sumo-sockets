using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [Header("Data")]
    [SerializeField]
    public Animator animator;
    private float speed = 4;

    private Vector3 desiredMoveDirection;


    [Header("Class references")]
    [SerializeField]
    private NetworkIdentity networkIdentity;
    public Renderer modelRenderer;
    public Rigidbody rb;
    public bool hitting;
    public bool hited;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (networkIdentity.IsBeingControlled())
        {
            Material[] material = modelRenderer.materials;
            material[3].color = Color.red;
            NetworkClient.instance.pm = this;
        }
        else
        {
            enabled = false;
        }
    }

    void Update () {
		if(networkIdentity.IsBeingControlled())
        {
            CheckMovement();
        }

        if (!hitting && Input.GetButtonDown("Jump"))
        {
            Hit();
        }
	} 

    public void Hit()
    {
        animator.Play("Hit");
        StartCoroutine(Hitting());
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + Vector3.up - transform.forward * 0.5f, 1f, transform.forward, out hit, 2))
        {
            NetworkClient.instance.Hit(transform.forward, hit.collider.GetComponent<NetworkIdentity>().id);
        }
        
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up  - transform.forward * 0.5f;
        float maxDistance = 2;
        float radius = 1;
        RaycastHit hit;

        bool isHit = Physics.SphereCast(origin, radius, transform.forward, out hit,
            maxDistance);
        if (isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(origin, transform.forward * hit.distance);
            Gizmos.DrawWireSphere(origin + transform.forward * hit.distance, radius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(origin, transform.forward * maxDistance);
        }
    }

    IEnumerator Hitting()
    {
        hitting = true;
        yield return new WaitForSeconds(0.9f);
        hitting = false;
    }
    
    IEnumerator Hited()
    {
        hited = true;
        yield return new WaitForSeconds(0.5f);
        hited = false;
    }

    private void CheckMovement()
    {
        var cam = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        desiredMoveDirection = forward * vertical + right * horizontal;

        if(desiredMoveDirection != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.1f);
    }

    public void GetHited(Vector3 force)
    {
        print("im being hited oh no");
        print(force);
        rb.AddForce(force * 5, ForceMode.Impulse);
        StartCoroutine(Hited());
    }
    
    

    private void FixedUpdate()
    {
        if (!hitting && !hited) rb.MovePosition(transform.position + desiredMoveDirection * Time.fixedDeltaTime * speed);
        animator.SetBool("isWalking", desiredMoveDirection != Vector3.zero);
    }
}
