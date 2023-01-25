using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SkeletonManager : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private float maxHp;
    [SerializeField] private float currentHp;
    [SerializeField] private float speed;
    [SerializeField] private float countdownTime;
    [SerializeField] private float currentTime;
    [SerializeField] private int random_attack;
    [SerializeField] private bool isLife;
    [SerializeField] private float exp;
    [SerializeField] private float money;


    private NavMeshAgent agent = null;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform player;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private PlayerManager p;
    public GameObject axeLeft;
    public GameObject axeRight;

    public Transform[] waypoints;
    public int waypointIndex;

    [SerializeField] private Image fillHpImage;
    [SerializeField] private Slider hpSlider;

    void Start()
    {
        isLife = true;
        currentHp = maxHp;
        waypointIndex = 0;
        agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (isLife == true)
        {
            HPbar();
            if (currentHp > 50 || currentHp <= 50 && waypointIndex == waypoints.Length)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                agent.SetDestination(player.position);
                RotateToTarget();

                float distanceToTarget = Vector3.Distance(transform.position, player.position);

                if (distanceToTarget <= stoppingDistance)
                {
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    RotateToTarget();
                    anim.SetBool("IsWalk", false);
                    if (currentTime > countdownTime)
                    {
                        AttackPlayer();
                        currentTime = 0;
                    }
                }
                else
                {
                    anim.SetBool("IsWalk", true);
                }

            }

            //hp less 50
            if (currentHp <= 50 && waypointIndex < waypoints.Length)
            {
                anim.SetBool("IsWalk", true);
                agent.SetDestination(waypoints[waypointIndex].position);
                if (Vector3.Distance(transform.position, waypoints[waypointIndex].position) < 5)
                {
                    IncreaseWaypoint();
                }
            }
        }
        else
        {
            anim.SetBool("IsLife", false);
        }

        anim.SetBool("GetHit", false);
    }

    private void RotateToTarget()
    {

        Vector3 direction = player.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rotation;
    }

    void IncreaseWaypoint()
    {
        waypointIndex++;
    }

    void AttackPlayer()
    {
        random_attack = Random.Range(1, 4);
        anim.SetInteger("Attack", random_attack);
        axeLeft.GetComponent<MeshCollider>().enabled = true;
        axeRight.GetComponent<MeshCollider>().enabled = true;
        anim.SetBool("IsWalk", false);
    }

    public int Damage()
    {
        if (random_attack == 1)
        {
            return 3;
        }

        if (random_attack == 2)
        {
            return 5;
        }

        if (random_attack == 3)
        {
            return 10;
        }

        else
        {
            return 0;
        }
    }

    public void SkeletonTakeDMG(int dmg)
    {
        p.sword.GetComponent<MeshCollider>().enabled = false;
        currentHp -= dmg;
        if (currentHp <= 0)
        {
            Dead();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sword")
        {
            anim.SetBool("GetHit", true);
            SkeletonTakeDMG(p.Damage());
            //Debug.Log("skeleton take dmg");
        }
    }

    private void HPbar()
    {
        if (hpSlider.value != 0)
        {
            float FillHpValue = currentHp / maxHp;
            hpSlider.value = FillHpValue;
        }
    }

    private void Dead()
    {
        isLife = false;
        ReturnEXPandMoney();
    }

    public void ReturnEXPandMoney()
    {
        p.GainEXPandMoney(exp, money);
    }
}
