using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float moveSpeed = 10;
    public float turnSpeed = 80;
    private Transform tr;
    private Animation anime;

    void Start()
    {
        tr = GetComponent<Transform>();
        anime = GetComponent<Animation>();

        anime.Play("Idle");
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //Debug.Log("h = " + h);
        //Debug.Log("v = " + v);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir * Time.deltaTime * moveSpeed);

        tr.Rotate(Vector3.up * Time.deltaTime * turnSpeed * r);

        PlayerAnime(h, v);
    }

    void PlayerAnime(float h, float v)
    {
        if (v >= 0.1f)
        {
            anime.CrossFade("RunF", 0.25f);
        }
        else if (v <= -0.1f)
        {
            anime.CrossFade("RunB", 0.25f);
        }
        else if (h >= 0.1f)
        {
            anime.CrossFade("RunR", 0.25f);
        }
        else if (h <= -0.1f)
        {
            anime.CrossFade("RunL", 0.25f);
        }
        else
        {
            anime.CrossFade("Idle", 0.25f);
        }
    }
}
