using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnplayerDie;

    public float currHp;
    public float moveSpeed = 10f;
    public float turnSpeed = 750f;
    private Transform tr;
    private Animation anime;

    private readonly float initHp = 100f;

    IEnumerator Start()
    {
        tr = GetComponent<Transform>();
        anime = GetComponent<Animation>();

        anime.Play("Idle");

        float temp = turnSpeed;
        turnSpeed = 0f;
        yield return new WaitForSeconds(.3f);
        turnSpeed = temp;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //Debug.Log("h = " + h);
        //Debug.Log("v = " + v);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveSpeed * Time.deltaTime * moveDir);

        tr.Rotate(r * Time.deltaTime * turnSpeed * Vector3.up);

        PlayerAnime(h, v);
    }

    void OnTriggerEnter(Collider other)
    {
        if (currHp >= 0f && other.CompareTag("Punch"))
        {
            currHp -= 10f;
            Debug.Log($"Player hp = {currHp / initHp}");

            if (currHp <= 0f)
            {
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        Debug.Log("Player die!");

        /*GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }*/
        OnplayerDie();
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
