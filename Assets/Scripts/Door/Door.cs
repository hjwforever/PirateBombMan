using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private Animator anim;
    private Collider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

        GameManager.GetInstance().SetDoor(this);
        coll.enabled = false;
    }

    public void OpenDoor()
    {
        anim.Play("Door_Open");
        coll.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            anim.Play("Door_Close");
            // NextScence();
        }
    }

    public void NextScence()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
            GameManager.GetInstance().NextScence();
    }
}