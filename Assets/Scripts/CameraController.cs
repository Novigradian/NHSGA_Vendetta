using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x + 6f,transform.position.y,transform.position.z);
        if (transform.position.x >= 7.75f)
        {
            transform.position = new Vector3(7.75f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= -7.75f)
        {
            transform.position = new Vector3(-7.75f, transform.position.y, transform.position.z);
        }
    }
}
