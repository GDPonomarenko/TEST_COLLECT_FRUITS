using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffController : MonoBehaviour
{
    private ParticleSystem particleSystemTrash;
    public GameObject charaterController;
    public Transform basketPos;
    public Transform handPos;
    public bool staffInHand;
    private Rigidbody rb;
    public ParticleSystem particleSystem;
    private bool isDestroy;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        charaterController = GameObject.Find("Character");
        particleSystemTrash = GameObject.Find("ParticleSystemTrash").GetComponent<ParticleSystem>();
        handPos = GameObject.Find("mixamorig:RightHandIndex1").GetComponent<Transform>();
        basketPos = GameObject.Find("TrowPosition").GetComponent<Transform>();
        StartCoroutine(DestroyStaff());
    }

    // Update is called once per frame
    void Update()
    {
        if (staffInHand) {
            transform.position = handPos.position;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Trash")
        {
            StartCoroutine(WaitDestroy());
        }
        if (collider.gameObject.tag == "Basket") {
            isDestroy = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Basket")
        {
            isDestroy = false;
            StartCoroutine(WaitDestroy());
        }
    }


    private IEnumerator WaitDestroy()
    {
        if (!isDestroy) {
            yield return new WaitForSeconds(2);
            particleSystemTrash.Play();
            Destroy(gameObject);
        }
        
    }


    private void OnMouseDown()
    {
        if (Vector3.Distance(transform.position, handPos.position)<2.5f) {
            charaterController.GetComponent<CharacterController>().selectOBJ = gameObject;
            charaterController.GetComponent<CharacterController>().HandUp();
            rb.freezeRotation = true;
        }
    }

    public void startCor() {
        StartCoroutine(MoveToHande());
    }
    public IEnumerator MoveToHande() {
        float needTime = 0.3f;
        float spentTime = 0;
        while (needTime > spentTime)
        {
            transform.position = Vector3.Lerp(transform.position, handPos.position, spentTime / needTime);
            spentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        staffInHand = true;
    }

    public void PlayParticle() {
        particleSystem.Play();
    }


    private IEnumerator DestroyStaff() {
        yield return new WaitForSeconds(40);
        Destroy(gameObject);
    }

}
