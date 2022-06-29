using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class detection : MonoBehaviour
{
    public Collider target;
    public Camera cam;
    public RectTransform cn;
    public RectTransform t_im;

    float minDistance = 9999f;
    // Start is called before the first frame update
    void Start()
    {
        minDistance = (transform.parent.position - target.gameObject.transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            float distance = (transform.parent.position - other.gameObject.transform.position).magnitude;
            if (distance < minDistance)
            {
                target = other;
                minDistance = distance;
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 screenPos = cam.WorldToScreenPoint(new Vector3(target.transform.position.x, target.transform.position.y + target.bounds.size.y / 2, target.transform.position.z));
        //Debug.Log(screenPos.y);
        t_im.anchoredPosition = new Vector2(screenPos.x*cn.rect.width/Screen.width, screenPos.y * cn.rect.height / Screen.height);

        minDistance = 9999f;
    }
}
