using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Osccliator : MonoBehaviour
{
    private float t = 0;
    [SerializeField]
    private Vector3 origin = Vector3.zero;
    [SerializeField]
    private float frequencyMultiplier = 3;
    [SerializeField]
    private float amplitudeMultiplier = 3;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = origin + new Vector3(amplitudeMultiplier * Mathf.Cos(t), 0, amplitudeMultiplier * Mathf.Sin(t));
        t += Time.deltaTime * frequencyMultiplier;
    }
}
