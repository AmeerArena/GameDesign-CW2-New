using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    private Rigidbody2D rb2d;
    private Animator animator;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");

        if (!PauseController.IsGamePaused)
        {
            rb2d.velocity = new Vector2(moveHorizontal, moveVertical) * moveSpeed;
            if (rb2d.velocity != Vector2.zero)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
        else
        {
            rb2d.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
    }
}
