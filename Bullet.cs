using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 0.3f;
    public Collider target;
    private bool crashed = false;
    public healthBar healthBar;
    public float bullet_power = 25;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!crashed)
        {
            Vector3 target_pos = new Vector3(target.transform.position.x, target.transform.position.y + target.bounds.size.y / 2, target.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target_pos, speed);
            if (target.GetComponent<EnemyController>().is_dead) Destroy(gameObject, 4);
        }
    }
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == target)
        {
            float hp = other.gameObject.GetComponent<EnemyController>().hp -= bullet_power;
            healthBar.Max = other.gameObject.GetComponent<EnemyController>().max_hp;
            healthBar.SetHealth(hp);
            if (hp <= 0)
            {
                other.gameObject.GetComponent<EnemyController>().is_dead = true;
            }
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
        //if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Ground")
        //{
            //crashed = true;
            //Destroy(gameObject, 4);
        //}
    }
}
