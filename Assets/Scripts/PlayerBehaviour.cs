using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Touch Input")]
    public Joystick joystick;
    [Range(0.01f, 1.0f)]
    public float sensitivity;

    [Header("Movement")]
    public float horizontalForce;
    public float verticalForce;
    public bool isGrounded;
    public bool isMoving;
    public Transform groundOrigin;
    public float groundRadius;
    public LayerMask groundLayerMask;
    [Range(0.1f, 0.9f)]
    public float airControlFactor;

    [Header("Animation")]
    public PlayerAnimationState state;


    private Rigidbody2D rigidbody;
    private Animator animatorController;

    [SerializeField]
    [Header("SoundFx")]
    public AudioClip jumpSound;
    public AudioClip footStep;
    public AudioClip hit;
    public AudioClip win;


    [SerializeField]
    private AudioSource musicSrc;

    private int health = 3;
    bool canMove = true;
    float timeLeft = 3.0f;
    bool collidingwithEnemy = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();
        musicSrc = GetComponent<AudioSource>();
       


    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        Move();
        CheckIfGrounded();
    }

    private void Move()
    {
        if (canMove)
        { float x = (Input.GetAxisRaw("Horizontal") + joystick.Horizontal) * sensitivity;

        if (isGrounded)
        {
            // Keyboard Input
            float y = (Input.GetAxisRaw("Vertical") + joystick.Vertical) * sensitivity;
            float jump = Input.GetAxisRaw("Jump") + ((UIController.jumpButtonDown) ? 1.0f : 0.0f);

            //jump activated
            if (jump > 0)
            {
                musicSrc.clip = jumpSound;
                musicSrc.Play();
                isMoving = false;
            }
            // Check for Flip

            if (x != 0)
                {
                    if (!collidingwithEnemy)
                    {
                        x = FlipAnimation(x);
                        animatorController.SetInteger("AnimationState", (int)PlayerAnimationState.RUN); // RUN State
                        state = PlayerAnimationState.RUN;
                        if(isGrounded)
                        isMoving = true;
                    }

            }
            else
            {
                    if (!collidingwithEnemy)
                    {
                        animatorController.SetInteger("AnimationState", (int)PlayerAnimationState.IDLE); // IDLE State
                        state = PlayerAnimationState.IDLE;
                        isMoving = false;
                    }
            }

            float horizontalMoveForce = x * horizontalForce;
            float jumpMoveForce = jump * verticalForce;

            float mass = rigidbody.mass * rigidbody.gravityScale;


            rigidbody.AddForce(new Vector2(horizontalMoveForce, jumpMoveForce) * mass);
            rigidbody.velocity *= 0.99f; // scaling / stopping hack
        }
        else // Air Control
        {
            animatorController.SetInteger("AnimationState", (int)PlayerAnimationState.JUMP); // JUMP State
            state = PlayerAnimationState.JUMP;

            if (x != 0)
            {
                x = FlipAnimation(x);

                float horizontalMoveForce = x * horizontalForce * airControlFactor;
                float mass = rigidbody.mass * rigidbody.gravityScale;

                rigidbody.AddForce(new Vector2(horizontalMoveForce, 0.0f) * mass);
            }
        } }
        if (isMoving)
        {
            if (!musicSrc.isPlaying)
            {
                musicSrc.clip = footStep;
                musicSrc.Play();
            }
           }
        else {
            if(musicSrc.clip==footStep)
            musicSrc.Stop();
        }
        if(health <= 0)
        {
            canMove = false;
            isMoving = false;
            timeLeft -= Time.deltaTime;
            animatorController.SetInteger("AnimationState", (int)PlayerAnimationState.DEAD); // JUMP State
            state = PlayerAnimationState.DEAD;
            if (timeLeft < 0)
            {
                SceneManager.LoadScene("GameOver");
            }
                Debug.Log(timeLeft);
        }

    }

    private void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.CircleCast(groundOrigin.position, groundRadius, Vector2.down, groundRadius, groundLayerMask);

        isGrounded = (hit) ? true : false;
    }

    private float FlipAnimation(float x)
    {
        // depending on direction scale across the x-axis either 1 or -1
        x = (x > 0.0f) ? 1.25f : -1.25f;

        transform.localScale = new Vector3(x, 1.25f);
        return x;
    }

    // EVENTS

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            musicSrc.clip = hit;
            animatorController.SetInteger("AnimationState", (int)PlayerAnimationState.HIT); 
            state = PlayerAnimationState.HIT;
           
            musicSrc.Play();
            if(health>0)
            PlayerHealth.healthValue--;
            health = PlayerHealth.healthValue;
            collidingwithEnemy = true;
        }

        if (other.gameObject.tag == "Door")
        {
            musicSrc.clip = win;
           

            musicSrc.Play();

            canMove = false;

            SceneManager.LoadScene("Level2");
        }
        if (other.gameObject.tag == "Door2")
        {
            musicSrc.clip = win;


            musicSrc.Play();

            canMove = false;

            SceneManager.LoadScene("MainMenu");
        }
       
    }

    private void OnCollisionExit2D(Collision2D other)
    {

        if (other.gameObject.tag == "Enemy")
        {
           
            collidingwithEnemy = false;
        }

       
    }

    // UTILITIES

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundOrigin.position, groundRadius);
    }

}
