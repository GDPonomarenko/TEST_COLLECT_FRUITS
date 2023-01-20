using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{
    [Header("MOVEMENT")]
    public float speedMovement;
    [HideInInspector] public Transform orientation;
    [HideInInspector] public Joystick joystickMove, joystickRotation;
    public float minDistaceToFruits;
    [HideInInspector] public Transform basketHand;
    [HideInInspector] public Transform rightHand;
    [HideInInspector] public Transform basketPosition;
    private Vector3 startRightHandPos;
    [HideInInspector] public Transform instBasketPos;
    Vector3 moveDirection;
    Rigidbody rb;
    private Animator animator;
    private bool upHend;
    [HideInInspector] public GameObject selectOBJ;
    [HideInInspector] public TMP_Text text;
    private int collect;
    private int needCollect;
    private string type;

    [Header("TYPE COLLECT OBJ")]
    public List<string> typeCollect;
    public GameObject camera;
    [HideInInspector] public Animator animatorCanvas;
    public Transform danceCameraPos;
    public Transform danceCameraLook;
    private bool isDance;

    [Header("WIN OBJECT")]
    public GameObject conveyerousSystem;
    public GameObject uiRestart;

    public AudioSource audioSource;
    public AudioClip runSound, collectSound;

    public List<GameObject> prefabs;
    public Transform instFruitsInBasket;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        startRightHandPos = rightHand.localPosition;

        GenerationNewCollect();

    }


    void FixedUpdate()
    {
        if (!isDance)
        {
            MovePlayer();
        }

    }

    private void OnAnimatorIK()
    {

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, basketHand.position);
        if (!isDance) {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        }
    }

    public void MovePlayer() {
        if (joystickMove.Vertical != 0 || joystickMove.Horizontal != 0)
        {
            if (joystickMove.Vertical > 0)
            {
                animator.SetBool("RunFront", true);
            }
            else {
                animator.SetBool("RunFront", false);
            }
            if (joystickMove.Vertical < 0)
            {
                animator.SetBool("RunBack", true);
            }
            else {
                animator.SetBool("RunBack", false);
            }
        }
        else {
            animator.SetBool("RunFront", false);
            animator.SetBool("RunBack", false);
        }

        moveDirection = orientation.forward * joystickMove.Vertical + orientation.right * joystickMove.Horizontal;
        rb.AddForce(moveDirection.normalized * speedMovement * 10f, ForceMode.Force);
        Vector3 targetLook = new Vector3(0, joystickRotation.Horizontal*5f, 0);
        transform.Rotate(targetLook);
    }

    public void HandUp() {

        if (selectOBJ != null) {
            if (selectOBJ.tag == type)
            {
                Collect();
            }
            if (Vector3.Distance(transform.position, selectOBJ.transform.position) < minDistaceToFruits)
            {
                if (!upHend)
                {
                    StartCoroutine(Take());
                    upHend = true;
                }
                else
                {
                    upHend = false;
                }
            }
            else
            {
                upHend = false;
            }
        }
    }


    private IEnumerator Take() {
        float needTime = 0.5f;
        float spentTime = 0;
        while(needTime > spentTime)
        {
            rightHand.position = Vector3.Lerp(rightHand.position, selectOBJ.transform.position, spentTime / needTime);
            spentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        selectOBJ.GetComponent<StaffController>().startCor();
        yield return new WaitForSeconds(0.5f);
        needTime = 0.5f;
        spentTime = 0;
        while (needTime > spentTime)
        {
            rightHand.position = Vector3.Lerp(rightHand.position, basketPosition.position, spentTime / needTime);
            spentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        selectOBJ.GetComponentInChildren<StaffController>().PlayParticle();
        PlayCollect();
        selectOBJ.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        for (int x =0; x < prefabs.Count; x++) {
            if (prefabs[x].tag == selectOBJ.tag) {
                Instantiate(prefabs[x], instFruitsInBasket.position, Quaternion.identity);
            }
        }
        Destroy(selectOBJ);
        yield return new WaitForEndOfFrame();
        rightHand.localPosition = new Vector3(startRightHandPos.x, 6.4f, startRightHandPos.z);    
    }


    public void GenerationNewCollect()
    {
        needCollect = Random.RandomRange(3, 6);
        collect = 0;
        type = typeCollect[Random.RandomRange(0, typeCollect.Count)];
        text.SetText("Collect " + type + "S " + collect.ToString() + "/" + needCollect.ToString());

    }

    public void Collect()
    {
        collect++;
        animator.Play(0);
        text.SetText("Collect "+type + "S " + collect.ToString() + "/" + needCollect.ToString());
        if (collect == needCollect)
        {
            text.SetText("Level Passed");
            StartCoroutine(DanceCamera());
            conveyerousSystem.SetActive(false);
            uiRestart.SetActive(true);
        }
    }

    private IEnumerator DanceCamera() {
        float needTime = 5f;
        float spentTime = 0;
        isDance = true;
        animator.SetBool("Dance", true);
        camera.GetComponent<CameraController>().dancePos = true;
        while (needTime > spentTime)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, danceCameraPos.position, spentTime / needTime);
            spentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(7);
        animator.SetBool("Dance", false);
        camera.GetComponent<CameraController>().dancePos = false;
        isDance = false;
    }


    public void RestartScene() {
        SceneManager.LoadScene(0);
    }


    public void PlayCollect() {
        audioSource.volume = 0.8f;
        audioSource.clip = collectSound;
        audioSource.pitch = Random.RandomRange(0.9f,1.1f);
        audioSource.Play();
    }

    public void RunSound() {
        audioSource.volume = 0.4f;
        audioSource.clip = runSound;
        audioSource.pitch = Random.RandomRange(0.9f, 1.1f);
        audioSource.Play();
    }
}
