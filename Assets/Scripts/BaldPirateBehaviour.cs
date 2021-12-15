using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaldPirateBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float movespeed = 1.0f;
    public float Lastmovespeed = 1.0f;



    private Animator animatorController;

    private Rigidbody2D rigidbody;

    [SerializeField]
    private AudioSource musicSrc;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        
        animatorController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(Vector3.right * movespeed * Time.deltaTime, Space.World);

    }
    private void Flip()
    {


        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        movespeed = movespeed * -1;

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Wall")
        {
            Flip();
        }
        if (other.gameObject.tag == "Player")
        {
            Lastmovespeed = movespeed;
            movespeed = 0;
            animatorController.SetTrigger("Attack");
            if (!musicSrc.isPlaying)
                musicSrc.Play();

        }
        else
        {
            if (musicSrc.isPlaying)
                musicSrc.Stop();
        }

    

}
    private void OnCollisionExit2D(Collision2D other)
    {

        if (other.gameObject.tag == "Player")
        {

            animatorController.SetTrigger("Run");
            movespeed = Lastmovespeed;

            Flip();

        }
    }
}
