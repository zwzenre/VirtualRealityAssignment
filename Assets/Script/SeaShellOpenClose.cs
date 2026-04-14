using UnityEngine;
using System.Collections;

public class SeaShellOpenClose : MonoBehaviour
{
    private Animator anim;

    [Header("Timing Settings")]
    public float minOpenTime = 2f;
    public float maxOpenTime = 5f;
    public float minClosedTime = 4f;
    public float maxClosedTime = 10f;

    void Start()
    {
        anim = GetComponent<Animator>();
        // Start the infinite loop
        StartCoroutine(SeashellRoutine());
    }

    IEnumerator SeashellRoutine()
    {
        while (true)
        {
            // 1. Wait while closed
            float waitClosed = Random.Range(minClosedTime, maxClosedTime);
            yield return new WaitForSeconds(waitClosed);

            // 2. Open the shell
            anim.SetTrigger("Open");

            // 3. Wait while open (Show off that pearl!)
            float waitOpen = Random.Range(minOpenTime, maxOpenTime);
            yield return new WaitForSeconds(waitOpen);

            // 4. Close the shell
            anim.SetTrigger("Close");
        }
    }
}