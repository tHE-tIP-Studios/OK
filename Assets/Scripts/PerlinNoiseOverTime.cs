using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseOverTime : MonoBehaviour
{
    private Vector3 debug;

    int seed;
    // Start is called before the first frame update
    void Start()
    {
        debug = transform.position;
        seed = Random.Range(0, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        debug.y = Mathf.PerlinNoise((Time.time + seed) * 2, 0);
        transform.position = debug;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, .5f);
    }
}