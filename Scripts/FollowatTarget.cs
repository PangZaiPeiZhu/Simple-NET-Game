using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowatTarget : MonoBehaviour
{
    
    GameObject target;
    public float speed=1f;
    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Myself");
        
        transform.position = Vector3.Lerp(transform.position, target.transform.position+new Vector3(-8, 6, -8), speed*Time.deltaTime);
    }
    private void FixedUpdate()
    {
        transform.LookAt(target.transform);
    }
}
