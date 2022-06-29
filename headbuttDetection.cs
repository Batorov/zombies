using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headbuttDetection : MonoBehaviour
{
    private EnemyController en;
    // Start is called before the first frame update
    void Start()
    {
        en = transform.parent.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (en.is_dead && other.gameObject.tag == "Ground")
        {
            en.headbutt = true;
        }
    }
}
