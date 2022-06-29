using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator anim;
    public PlayerController player;
    public bool is_dead = false;
    public Transform[] points;
    private Transform target_point;
    private int target_point_ind = 0;
    private Rigidbody rb;
    private Collider coll;
    public float speed = 2f;
    private Vector3 last_life_pos;
    private SkinnedMeshRenderer cube;
    public Material mater;
    public bool headbutt = false;
    public float hp = 100;
    public float potent_hp = 100;
    public float max_hp = 100;
    public float enemy_radius_vision = 6f;
    public float enemy_radius_attack = 3f;
    public LayerMask eneme_agr_mask;
    public LayerMask obstMask;
    public Transform fov_point;
    private bool isStay = false;
    private bool isBypass = false;
    private bool isAttack = false;
    private IEnumerator attack_coroutine;
    private bool ch_attack_coroutine = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        attack_coroutine = Attack();
        anim.Play("Base Layer.Armature|idle", 0, Random.Range(0.0f, 1.0f));
        if (points.Length !=0) target_point = points[target_point_ind];
        cube = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!is_dead)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemy_radius_vision, eneme_agr_mask);
            if (hitColliders.Length != 0 && !hitColliders[0].GetComponent<PlayerController>().is_dead)
            {

                Vector3 targetPos = new Vector3(hitColliders[0].transform.position.x, hitColliders[0].transform.position.y + hitColliders[0].bounds.size.y / 2, hitColliders[0].transform.position.z);
                Vector3 directionToTarget = (targetPos - fov_point.position).normalized;
                float distance = Vector3.Distance(fov_point.position, targetPos);
                if (!Physics.Raycast(fov_point.position, directionToTarget, distance, obstMask))
                {
                    target_point = hitColliders[0].transform;
                    isBypass = false;
                    if (enemy_radius_attack >= distance)
                    {
                        //Debug.Log(distance);
                        isAttack = true;
                    }
                    else
                    {
                        isAttack = false;
                    }
                }
                else
                {
                    isAttack = false;
                    if (!isBypass)
                    {
                        if (points.Length == 0 || isStay)
                        {
                            target_point = null;

                        }
                        else
                        {
                            target_point = points[target_point_ind];
                        }
                    }
                }
            }
            else
            {
                isAttack = false;
                if (!isBypass)
                {
                    if (points.Length == 0 || isStay) { target_point = null; } else { target_point = points[target_point_ind]; }
                }
            }
            if (target_point && target_point.tag == "Player")
            {
                if (!target_point.GetComponent<PlayerController>().is_dead)
                {
                    if (isAttack)
                    {
                        anim.SetBool("Attack", true);

                        if (!ch_attack_coroutine)
                        {
                            ch_attack_coroutine = true;
                            StartCoroutine(attack_coroutine);
                        }
                    }
                    else
                    {
                        anim.SetBool("Attack", false);
                        StopCoroutine(attack_coroutine);
                        ch_attack_coroutine = false;
                    }
                }
                else
                {
                    target_point = points[target_point_ind];
                    anim.SetBool("Attack", false);
                    StopCoroutine(attack_coroutine);
                    ch_attack_coroutine = false;
                }
            }
            if (target_point && target_point.tag != "Player" && ch_attack_coroutine)
            {
                anim.SetBool("Attack", false);
                StopCoroutine(attack_coroutine);
                ch_attack_coroutine = false;
            }
        } else
        {
            if (ch_attack_coroutine)
            {
                anim.SetBool("Attack", false);
                StopCoroutine(attack_coroutine);
                ch_attack_coroutine = false;
            }
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            float hp = target_point.GetComponent<PlayerController>().hp -= 20;
            target_point.GetComponent<PlayerController>().playerHealthBar.SetHealth(hp);
            yield return new WaitForSeconds(0.5f);
            
        }
        //ch_attack_coroutine = false;
    }
    private void FixedUpdate()
    {
        if (is_dead)
        {
            rb.velocity = new Vector3(0, 0, 0);
            if (!headbutt) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(last_life_pos), Time.deltaTime * speed);
            //rb.AddTorque(transform.right * 10.0f, ForceMode.VelocityChange);
            anim.SetBool("Walk", false);
            cube.material = mater;
            coll.isTrigger = true;
            Destroy(gameObject, 4f);
        }
        else {

            last_life_pos = new Vector3(transform.forward.x, 90, transform.forward.z);
            if (target_point)
            {
                anim.SetBool("Walk", true);
                float deltaX = target_point.position.x - transform.position.x;
                float deltaZ = target_point.position.z - transform.position.z;
                float angle = Mathf.Atan2(deltaZ, deltaX);
                rb.velocity = new Vector3(speed * Mathf.Cos(angle), rb.velocity.y, speed * Mathf.Sin(angle));

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(speed * Mathf.Cos(angle), 0, speed * Mathf.Sin(angle))), Time.deltaTime * speed);
            } else
            {
                anim.SetBool("Walk", false);
            }
        }
    }

    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Movement point" && other.transform == target_point)
        {
            if (target_point_ind == points.Length - 1) target_point_ind = 0; else target_point_ind++;
            target_point = points[target_point_ind];
        }
        if (other.tag == "Stay point" && other.transform == target_point)
        {
            //if (target_point_ind == points.Length - 1) target_point_ind = 0; else target_point_ind++;
           
            isStay = true; 
            //Debug.Log(isStay);
        }
        
        if (other.tag == "Bypass point" && other.transform == target_point)
        {
            //if (target_point_ind == points.Length - 1) target_point_ind = 0; else target_point_ind++;
            target_point = points[target_point_ind];
            isBypass = false;
            //Debug.Log("h");
        }
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            //target_point = other.gameObject.GetComponent<Bypass>().points[0];
            float mindist = 9999f;
            foreach(Transform tr in other.gameObject.GetComponent<Bypass>().points)
            {
                if ((tr.position-points[target_point_ind].position).magnitude < mindist)
                {
                    mindist = (tr.position - points[target_point_ind].position).magnitude;

                    //Debug.Log(tr.name+" "+mindist);
                    target_point = tr;
                }
            }
            isBypass = true;
            //Debug.Log(target_point.name);
           // Debug.Log("i");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Stay point" && other.name == points[target_point_ind].name)
        {
            //if (target_point_ind == points.Length - 1) target_point_ind = 0; else target_point_ind++;

            isStay = false; 

        }
    }
}
