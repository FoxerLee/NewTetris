using System;
using UnityEngine;

public class Obj : MonoBehaviour
{
    public int id;
    public int score;
    public GameObject nextLevelPrefab;
    public Action<Obj, Obj> OnLevelUp;
    public Action OnGameWin;

    private Rigidbody2D rigid;
    private bool isTouchRedline;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTouchRedline == false)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer > 5)
        {
            Debug.Log("Win!");
            OnGameWin?.Invoke();
        }
    }

    public void SetSimulated(bool b)
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        rigid.simulated = b;
        rigid.velocity = new Vector3(0, -4, 0);
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     var obj = collision.gameObject;
    //     var obj_old = obj.GetComponent<Obj>();
    //     if (obj.CompareTag("Obj"))
    //     {   
    //         if (obj.name == gameObject.name)
    //         {
    //             if (nextLevelPrefab != null)
    //             {
    //                 OnLevelUp?.Invoke(this, obj_old);
    //             }
    //         }
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            // Debug.Log("OnTriggerEnter2D Redline");
            isTouchRedline = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            // Debug.Log("OnTriggerExit2D Redline");
            isTouchRedline = false;
            timer = 0;
        }
    }

}
