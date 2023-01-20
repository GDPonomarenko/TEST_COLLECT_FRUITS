using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectController : MonoBehaviour
{
    public TMP_Text text;
    public int collect;
    public int needCollect;
    public string type;
    public List<string> typeCollect;
    public Camera camera;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        collect = 0;
        needCollect = 5;
        type = "APPLE";
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(type + " " + collect.ToString() + "/" + needCollect.ToString());

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == type) {
                    Collect();
                }
            }
        }
    }

    public void GenerationNewCollect() {
        needCollect = Random.RandomRange(3, 6);
        collect = 0;
        type = typeCollect[Random.RandomRange(0, typeCollect.Count)];
    }

    public void Collect() {
        collect++;
        animator.Play(0);
        if (collect == needCollect) {
            GenerationNewCollect();
        }
    }
}
