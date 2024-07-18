using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public float moveSpeed = 2f; // Adjust this to set the character's movement speed
    public float jumpForce = 2f; // Adjust this to set the character's jump force

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool Finish = false;
    private bool enableClimb = false;
    private bool crouched = false;
    private Vector3 initialPlayerPosition;
    private int attackDurationCounter;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPlayerPosition = GameObject.FindWithTag("Player").transform.position;
        attackDurationCounter = 0;
    }
    void Update()
    {
        if (!Finish)
        {
            AttackController();
            ClimbableColliderController();
            WalkInDirection();
            Jump();
            Crouch();
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the character is touching any collider
        foreach (ContactPoint2D contact in collision.contacts)
        {
            RespawnAndFinishController(contact);
            if (JumpController(contact)) { break; }
        }
    }
    void AttackController()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetBool("Attack", true);
            attackDurationCounter = 50;
        }
        if (attackDurationCounter != 0)
        {
            attackDurationCounter--;
        }
        if (attackDurationCounter == 0)
        {
            animator.SetBool("Attack", false);
        }
    }
    void ClimbableColliderController()
    {
        if (Input.GetKeyDown(KeyCode.C)) //Toggle State if C is pressed
        {
            enableClimb = !enableClimb;
        }
        // Find all GameObjects with the "ClimbAble" tag
        GameObject[] climbables = GameObject.FindGameObjectsWithTag("ClimbAble");

        // Loop through each GameObject and toggle colliders
        foreach (GameObject obj in climbables)
        {
            Collider2D[] colliders = obj.GetComponents<Collider2D>();

            // Toggle colliders on/off
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = enableClimb;
            }
        }
    }
    void WalkInDirection()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }
        if (moveInput < 0 && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput > 0 && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
        }
        if (crouched)
        {
            moveInput = 0;
        }
        // Apply movement
        Vector2 movement = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;
    }
    void Jump()
    {
        // Check if the character can jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !crouched)
        {
            animator.SetBool("JumpAnim", true);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
    }
    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            crouched = true;
            animator.SetBool("Walk", !crouched);
            animator.SetBool("JumpAnim", !crouched);
            animator.SetBool("Attack", !crouched);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            crouched = false;
        }
        animator.SetBool("Crouch", crouched);
    }
    bool JumpController(ContactPoint2D contact)
    {
        if (contact.collider.tag == "Ground" || contact.collider.tag == "ClimbAble") // Replace "Ground" with your tag or condition for jumpable surfaces
        {
            isGrounded = true;
            animator.SetBool("JumpAnim", false);
            return true;
        }
        return false;
    }
    void RespawnAndFinishController(ContactPoint2D contact)
    {
        if (contact.collider.tag == "LethalEdge")
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = initialPlayerPosition;
        }
        if (contact.collider.tag == "Finish")
        {
            animator.SetBool("Win", true);
            Finish = true;
        }
    }
}