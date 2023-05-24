using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.IDLE;

    public float traceDist = 10.0f;

    public float attackDist = 2.0f;

    public bool isDie= false;

    private GameObject bloodEffect;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anime;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");

    private readonly int hashHit = Animator.StringToHash("Hit");

    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    private readonly int hashSpeed = Animator.StringToHash("Speed");

    private readonly int hashDie = Animator.StringToHash("Die");

    private int hp = 100;

    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");

        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();

        anime = GetComponent<Animator>();

        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            hp -= 10;
            if (hp <= 0)
            {
                state = State.DIE;
            }

            Vector3 pos = collision.GetContact(0).point;
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            ShowBloodEffect(pos, rot);

            Destroy(collision.gameObject);
            anime.SetTrigger(hashHit);
        }
    }

    void OnEnable()
    {
        PlayerCtrl.OnplayerDie += OnPlayerDie;
    }

    void OnDisable()
    {
        PlayerCtrl.OnplayerDie -= OnPlayerDie;
    }

    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        GameObject blood = Instantiate(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1f);
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        agent.isStopped = true;
        anime.SetFloat(hashSpeed, Random.Range(.8f, 1.2f));
        anime.SetTrigger(hashPlayerDie);
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);
            if (state == State.DIE)
                yield break;

            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;

                    anime.SetBool(hashTrace, false);
                    break;

                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;

                    anime.SetBool(hashAttack, false);
                    anime.SetBool(hashTrace, true);
                    break;

                case State.ATTACK:
                    anime.SetBool(hashAttack, true);
                    break;

                case State.DIE:
                    Debug.Log("Monster is die");

                    isDie = true;
                    agent.isStopped = true;
                    anime.SetTrigger(hashDie);

                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
            yield return new WaitForSeconds(.3f);
        }
    }

    void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }

        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }
}