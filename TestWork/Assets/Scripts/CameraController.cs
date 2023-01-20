using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform lookTarget;
    private Transform _transform;
    public float xDistance, yDistance, zDistance;
    Vector3 newPos;
    public bool dancePos;

    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (!dancePos) {
            newPos = new Vector3(lookTarget.position.x + xDistance, lookTarget.position.y + yDistance, lookTarget.position.z + zDistance);
            _transform.position = Vector3.Lerp(_transform.position, newPos, Time.deltaTime * 3);
        }
        _transform.LookAt(lookTarget);
    }
}
