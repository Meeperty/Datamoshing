using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDMovement : MonoBehaviour
{
    public float speed;

    private Transform m_transform;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = m_transform.position + new Vector3(0,0,speed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position = m_transform.position + new Vector3(0,0,-speed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position = m_transform.position + new Vector3(speed,0,0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position = m_transform.position + new Vector3(-speed,0,0);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
