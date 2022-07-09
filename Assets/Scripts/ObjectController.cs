using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public Rigidbody rb;
    public bool dragging = false;
    public float rotationSpeed;

    public GameObject[] animalsObj;
    public bool[] isAnimal = new bool[] { true, true, true };

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        AnimalsActive();
    }

    void Update()
    {
        //Controller();
        Drag();

    }

    public void AnimalsActive()
    {
        if(!isAnimal[0])
        {
            animalsObj[0].SetActive(false);
        }
        else
            animalsObj[0].SetActive(true);

        if (!isAnimal[1])
        {
            animalsObj[1].SetActive(false);
        }
        else
            animalsObj[1].SetActive(true);

        if (!isAnimal[2])
        {
            animalsObj[2].SetActive(false);
        }
        else
            animalsObj[2].SetActive(true);
    }

    private void OnMouseDrag()
    {
        dragging = true;
    }

    public void Controller()
    {
        if(Input.touchCount >0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;

            if(Physics.Raycast(ray,out hit))
            {
                if(hit.collider.tag == "Ground")
                {
                    dragging = true;
                }
            }
        }

#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Ground")
                {
                    dragging = true;
                }
            }
        }
#endif
    }

    public void Drag()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            dragging = false;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            dragging = false;
        }
#endif
    }

    private void FixedUpdate()
    {
        if(dragging)
        {
            float x = Input.GetAxis("Mouse X") * rotationSpeed * Time.fixedDeltaTime;
            float y = Input.GetAxis("Mouse Y") * rotationSpeed * Time.fixedDeltaTime;

            rb.AddTorque(Vector3.down * x);
            rb.AddTorque(Vector3.right * y);
        }
    }
}
