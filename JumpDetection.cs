using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    public PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    private void OnTriggerExit(Collider other)
    {

        if (pc.is_grounded && (other.gameObject.tag == "Ground" || other.gameObject.tag == "Wall"))
        {
            pc.is_grounded = false;
        }
    }

    private void OnTriggerStay(Collider collisionInfo)
    {
        if (!pc.is_grounded && (collisionInfo.gameObject.tag == "Ground" || collisionInfo.gameObject.tag == "Wall"))
        {
            pc.is_grounded = true;
        }
    }
}
