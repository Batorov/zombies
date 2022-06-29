using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    //public Collider pistol_coll;
    private float speed = 5f;
    private float moveInputHor;
    private float moveInputVer;
    private float cs;
    private float sn;
    private Vector3 pose;
    public bool is_grounded;
    private float jump_force = 8f;
    private bool attack = false;
    public GameObject detect;
    private Collider target;
    //private Collider oldtarget;
    public GameObject bullet;
    public GameObject bullet_point;
    public GameObject new_bullet;
    private IEnumerator coroutine;
    private bool isFirstShot = true;
    public RectTransform cn;
    public RectTransform t_im;
    public LayerMask targetMask;
    public Transform fov_point;
    public LayerMask obstMask;
    private Animator anim;
    public healthBar enemyHealthBar;
    public healthBar playerHealthBar;
    public RectTransform eHB_transform;
    public float hp = 100;
    public bool is_dead = false;
    private Vector3 last_life_pos;

    public Joystick joystick;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        anim = GetComponent<Animator>();
        coroutine = spawnBullets();
        cs = Mathf.Cos(-Camera.main.transform.localRotation.eulerAngles.y * Mathf.PI / 180);
        sn = Mathf.Sin(-Camera.main.transform.localRotation.eulerAngles.y * Mathf.PI / 180);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!is_dead)
        {
            //target = detect.GetComponent<detection>().target;
            if (target)
            {
                Vector3 targetpos = new Vector3(-transform.position.x + target.transform.position.x, 0, -transform.position.z + target.transform.position.z);
                if (attack) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetpos), Time.deltaTime * speed);
            }
            moveInputHor = joystick.Horizontal;
            moveInputVer = joystick.Vertical;

            //Debug.Log(Camera.main.transform.localRotation.eulerAngles.y);
            float moveHor = moveInputHor * cs - moveInputVer * sn;
            float moveVer = moveInputHor * sn + moveInputVer * cs;
            rb.velocity = new Vector3(moveHor * speed, rb.velocity.y, moveVer * speed);
            pose = new Vector3(moveHor * speed, 0, moveVer * speed).normalized;
            if (moveInputHor != 0 || moveInputVer != 0)
            {
                anim.SetBool("shot_idle", false);
                if (attack)
                {
                    anim.SetBool("walk", false);
                    anim.SetBool("shot_walk", true);
                }
                else
                {
                    anim.SetBool("shot_walk", false);
                    anim.SetBool("walk", true);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pose), Time.deltaTime * speed);
                }
            }
            else
            {
                anim.SetBool("walk", false);
                anim.SetBool("shot_walk", false);
                if (attack) { anim.SetBool("shot_idle", true); } else { anim.SetBool("shot_idle", false); }
            }
        }
    }

    private void Update()
    {
        if (!is_dead)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 12, targetMask);
            if (hitColliders.Length == 0)
            {
                target = null;
                t_im.anchoredPosition = new Vector2(-100, -100);
                eHB_transform.anchoredPosition = new Vector2(-100, -100);
            }
            else
            {
                List<Collider> hits = new List<Collider>();

                foreach (var hitCollider in hitColliders)
                {
                    if (!hitCollider.GetComponent<EnemyController>().is_dead)
                    {
                        Vector3 targetPos = new Vector3(hitCollider.transform.position.x, hitCollider.transform.position.y + hitCollider.bounds.size.y / 2, hitCollider.transform.position.z);
                        Vector3 directionToTarget = (targetPos - fov_point.position).normalized;
                        float distance = Vector3.Distance(fov_point.position, targetPos);
                        if (!Physics.Raycast(fov_point.position, directionToTarget, distance, obstMask))
                        {
                            hits.Add(hitCollider);
                        }
                    }
                }
                float min_dist = 9999f;
                if (hits.Count != 0)
                {
                    //oldtarget = target;
                    foreach (var hitCollider in hits)
                    {
                        float distance = (fov_point.position - hitCollider.transform.position).magnitude;
                        if (distance < min_dist)
                        {
                            min_dist = distance;
                            target = hitCollider;
                        }
                    }
                    enemyHealthBar.SetHealth(target.GetComponent<EnemyController>().hp);

                }
                else
                {
                    target = null;
                    t_im.anchoredPosition = new Vector2(-100, -100);
                    eHB_transform.anchoredPosition = new Vector2(-100, -100);
                }
            }
            
            last_life_pos = new Vector3(transform.forward.x, 90, transform.forward.z);
        }
        if (hp <= 0)
        {
            
            
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(last_life_pos), Time.deltaTime * 2f);
            //pistol_coll.isTrigger = true;
            if (!is_dead)
            {
                anim.SetBool("shot_walk", false);
                anim.SetBool("shot_idle", false);
                anim.SetBool("walk", false);
                anim.SetTrigger("death");
                rb.velocity = new Vector3(0, 0, 0);
                endAttack();
                
            }
            is_dead = true;
        }

    }
    private void LateUpdate()
    {
        if (target && !is_dead) { 
            Vector2 screenPos = Camera.main.WorldToScreenPoint(new Vector3(target.transform.position.x, target.transform.position.y + target.bounds.size.y / 2, target.transform.position.z));
            Vector2 hbscreenPos = Camera.main.WorldToScreenPoint(new Vector3(target.transform.position.x, target.transform.position.y + target.bounds.size.y + 1, target.transform.position.z));
            //Debug.Log(screenPos.y);
            //float sp = 0.5f;
            Vector2 newAnchscreenPos = new Vector2(screenPos.x * cn.rect.width / Screen.width, screenPos.y * cn.rect.height / Screen.height);
            //if (oldtarget == target)
            //{
              //  t_im.anchoredPosition = Vector2.Lerp(t_im.anchoredPosition, newAnchscreenPos, sp);
               // eHB_transform.anchoredPosition = Vector2.Lerp(eHB_transform.anchoredPosition, new Vector2(hbscreenPos.x * cn.rect.width / Screen.width, hbscreenPos.y * cn.rect.height / Screen.height), sp);
            //} else
            //{
                t_im.anchoredPosition = newAnchscreenPos;
                eHB_transform.anchoredPosition = new Vector2(hbscreenPos.x * cn.rect.width / Screen.width, hbscreenPos.y * cn.rect.height / Screen.height);
            //}
        } else
        {
            t_im.anchoredPosition = new Vector2(-100, -100);
            eHB_transform.anchoredPosition = new Vector2(-100, -100);
        }
    }
    public void jump()
    {
        if (is_grounded && !is_dead)
        {
            rb.velocity = new Vector3(rb.velocity.x, jump_force, rb.velocity.z);
            is_grounded = false;
        }
    }
    

    IEnumerator spawnBullets()
    {
        while (true)
        {
            if (target)
            {
                bool is_dead_potent = false;
                if (target.GetComponent<EnemyController>().potent_hp <= 0) is_dead_potent = true;
                if (!isFirstShot && !is_dead_potent && !target.gameObject.GetComponent<EnemyController>().is_dead)
                {
                    target.GetComponent<EnemyController>().potent_hp -= 25;
                    new_bullet = Instantiate(bullet, bullet_point.transform.position, Quaternion.identity);
                    new_bullet.GetComponent<Bullet>().target = target;
                    new_bullet.GetComponent<Bullet>().healthBar = enemyHealthBar;
                }
            }
            isFirstShot = false;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void startAttack()
    {

        if (target && !is_dead)
        {
            attack = true;
            StartCoroutine(coroutine);
        }
    }


    public void endAttack()
    {
        attack = false;
        isFirstShot = true;
        StopCoroutine(coroutine);
    }
}
