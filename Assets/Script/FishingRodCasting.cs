using UnityEngine;

public class FishingRodCasting : MonoBehaviour
{
    public Transform rodTip;
    public GameObject hookPrefab;
    public float castForce = 15f;
    public float maxLineLength = 10f;

    private GameObject currentHook;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void Cast()
    {
        currentHook = Instantiate(hookPrefab, rodTip.position, rodTip.rotation);

        Rigidbody rb = currentHook.GetComponent<Rigidbody>();
        rb.AddForce(rodTip.forward * castForce, ForceMode.Impulse);
    }

    void Update()
    {
        if (currentHook != null)
        {
            line.SetPosition(0, rodTip.position);
            line.SetPosition(1, currentHook.transform.position);

            float distance = Vector3.Distance(rodTip.position, currentHook.transform.position);

            if (distance > maxLineLength)
            {
                Vector3 direction = (currentHook.transform.position - rodTip.position).normalized;
                currentHook.transform.position = rodTip.position + direction * maxLineLength;
            }
        }
    }
}
