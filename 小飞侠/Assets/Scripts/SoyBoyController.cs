/*
 * Copyright (c) 2016 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))]
public class SoyBoyController : MonoBehaviour {

    public float speed = 14f;
    public float accel = 6f;
    private Vector2 input;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;
    public bool isJumping;
    public float jumpSpeed = 8f;
    private float rayCastLengthCheck = 0.005f;
    private float width;
    private float height;
    public float jumpDurationThreshold = 0.25f;
    private float jumpDuration;
    public float airAccel = 3f;
    public float jump = 14f;
    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip slideClip;
    private AudioSource audioSource;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        width = GetComponent<Collider2D>().bounds.extents.x + 0.1f;
        height = GetComponent<Collider2D>().bounds.extents.y + 0.2f;
        audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
	
	}

    void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            if (!audioSource.isPlaying) audioSource.PlayOneShot(clip);
        }
    }

    public bool PlayerIsOnGround()
    {
        bool groundCheck1 = Physics2D.Raycast(new Vector2(
          transform.position.x, transform.position.y - height),
          -Vector2.up, rayCastLengthCheck);
        bool groundCheck2 = Physics2D.Raycast(new Vector2(
          transform.position.x + (width - 0.2f),
          transform.position.y - height), -Vector2.up,
          rayCastLengthCheck);
        bool groundCheck3 = Physics2D.Raycast(new Vector2(
          transform.position.x - (width - 0.2f),
          transform.position.y - height), -Vector2.up,
          rayCastLengthCheck);
        if (groundCheck1 || groundCheck2 || groundCheck3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsWallToLeftOrRight()
    {
        bool wallOnleft = Physics2D.Raycast(new Vector2(
          transform.position.x - width, transform.position.y),
          -Vector2.right, rayCastLengthCheck);
        bool wallOnRight = Physics2D.Raycast(new Vector2(
          transform.position.x + width, transform.position.y),
          Vector2.right, rayCastLengthCheck);
        if (wallOnleft || wallOnRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerIsTouchingGroundOrWall()
    {
        if (PlayerIsOnGround() || IsWallToLeftOrRight())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetWallDirection()
    {
        bool isWallLeft = Physics2D.Raycast(new Vector2(
          transform.position.x - width, transform.position.y),
          -Vector2.right, rayCastLengthCheck);
        bool isWallRight = Physics2D.Raycast(new Vector2(
          transform.position.x + width, transform.position.y),
          Vector2.right, rayCastLengthCheck);
        if (isWallLeft)
        {
            return -1;
        }
        else if (isWallRight)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Jump");
        animator.SetFloat("Speed", Mathf.Abs(input.x));
        if (input.x > 0f)
        {
            sr.flipX = false;
        }
        else if (input.x < 0f)
        {
            sr.flipX = true;
        }

        if (input.y >= 1f)
        {
            jumpDuration += Time.deltaTime;
            animator.SetBool("IsJumping", true);
        }
        else
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
            jumpDuration = 0f;
        }

        if (PlayerIsOnGround() && !isJumping)
        {
            if (input.y > 0f)
            {
                isJumping = true;
                PlayAudioClip(jumpClip);
            }
            animator.SetBool("IsOnWall", false);
            if (input.x < 0f || input.x > 0f)
            {
                PlayAudioClip(runClip);
            }
        }

        if (jumpDuration > jumpDurationThreshold) input.y = 0f;
    }

    void FixedUpdate()
    {
        var acceleration = 0f;
        if (PlayerIsOnGround())
        {
            acceleration = accel;
        }
        else
        {
            acceleration = airAccel;
        }
        var xVelocity = 0f;
        if (PlayerIsOnGround() && input.x == 0)
        {
            xVelocity = 0f;
        }
        else
        {
            xVelocity = rb.velocity.x;
        }
        var yVelocity = 0f;
        if (PlayerIsTouchingGroundOrWall() && input.y == 1)
        {
            yVelocity = jump;
        }
        else
        {
            yVelocity = rb.velocity.y;
        }
        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x)
          * acceleration, 0));
        rb.velocity = new Vector2(xVelocity, yVelocity);

        if (IsWallToLeftOrRight() && !PlayerIsOnGround()
            && input.y == 1)
        {
            rb.velocity = new Vector2(-GetWallDirection() * speed
              * 0.75f, rb.velocity.y);
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
            PlayAudioClip(jumpClip);
        }
        else if (!IsWallToLeftOrRight())
        {
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
        }
        if (IsWallToLeftOrRight() && !PlayerIsOnGround())
        {
            animator.SetBool("IsOnWall", true);
            PlayAudioClip(slideClip);
        }

        if (isJumping && jumpDuration < jumpDurationThreshold)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }
}
