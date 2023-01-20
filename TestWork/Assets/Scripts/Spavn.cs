using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spavn : MonoBehaviour
{
    public Transform spavnPos;
    public List<GameObject> prefab;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private IEnumerator Spawn() {
        while (true) {
            audioSource.pitch = Random.RandomRange(0.9f,1.1f);
            audioSource.Play();
            Instantiate(prefab[Random.RandomRange(0,prefab.Count)], spavnPos.position, Quaternion.identity);
            yield return new WaitForSeconds(Random.RandomRange(2f,4f));
        }
    }
}
