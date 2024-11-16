using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDeleter : MonoBehaviour
{
    public GameObject platformDeletionPoint;

    // Start is called before the first frame update
    void Start()
    {
        platformDeletionPoint = GameObject.Find("PlatformDeletionPoint");
    }

    // Update is called once per frame
    void Update()
    {
        if (platformDeletionPoint!=null && transform.position.x < platformDeletionPoint.transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
