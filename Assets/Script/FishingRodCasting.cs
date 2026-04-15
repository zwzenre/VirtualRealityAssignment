using UnityEngine;

public class FishingRodCasting : MonoBehaviour
{
    public Transform rodTip;
    public GameObject hookPrefab;
    public LineRenderer line;
    public GameObject fishPrefab;

    public float castForce = 20f;
    public float reelSpeed = 5f;
    public float maxLineLength = 15f;
    private float currentLineLimit;

    private GameObject currentHook;
    private Rigidbody hookRb;
    private GameObject currentFish;
    private bool isCast = false;

    void Update()
    {
        if (isCast && currentHook != null)
        {
            UpdateLine();
        }
    }

    void FixedUpdate()
    {
        if (isCast && currentHook != null)
        {
            ApplyLineTension();
        }
    }

    void CastLine()
    {
        ResetCast();
        currentLineLimit = maxLineLength;
        currentHook = Instantiate(hookPrefab, rodTip.position, rodTip.rotation);
        hookRb = currentHook.GetComponent<Rigidbody>();
        hookRb.AddForce(rodTip.forward * castForce, ForceMode.Impulse);
        hookRb.linearDamping = 1.5f;
        isCast = true;
    }

    void ApplyLineTension()
    {
        Vector3 direction = currentHook.transform.position - rodTip.position;
        float distance = direction.magnitude;

        if (distance > currentLineLimit)
        {
            float pullStrength = (distance - currentLineLimit) * 150f;
            hookRb.AddForce(-direction.normalized * pullStrength);
            hookRb.linearVelocity *= 0.95f;
        }
    }

    void UpdateLine()
    {
        int segments = 20;
        line.positionCount = segments;
        float tension = GetTension();

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 pos = Vector3.Lerp(rodTip.position, currentHook.transform.position, t);
            float sag = Mathf.Sin(t * Mathf.PI) * (1f - tension) * 0.5f;
            pos += Vector3.down * sag;
            line.SetPosition(i, pos);
        }
    }

    public float GetTension()
    {
        if (currentHook == null) return 0;
        float dist = Vector3.Distance(rodTip.position, currentHook.transform.position);

        float slack = currentLineLimit - dist;

        float slackThreshold = 2f;

        if (slack > slackThreshold)
        {
            return 0f;
        }

        float tension = 1f - (slack / slackThreshold);
        tension = Mathf.Pow(tension, 1.5f);
        return Mathf.Clamp01(tension);
    }

    public void SimulateFish()
    {
        if (hookRb != null)
        {
            Vector3 struggle = new Vector3(Mathf.Sin(Time.time * 5f), 0, Mathf.Cos(Time.time * 5f));
            hookRb.AddForce(struggle * 10f, ForceMode.Force);
        }
    }

    public void SpawnFish()
    {
        if (currentHook != null && currentFish == null)
        {
            currentFish = Instantiate(fishPrefab, currentHook.transform.position, Quaternion.identity);
            currentFish.transform.SetParent(currentHook.transform);
        }
    }

    public void ResetCast()
    {
        isCast = false;
        if (currentHook) Destroy(currentHook);
        if (currentFish) Destroy(currentFish);
        line.positionCount = 0;
    }

    public void Cast()
    {
        CastLine();
    }
    public void Reel()
    {
        currentLineLimit -= reelSpeed * Time.deltaTime;
        currentLineLimit = Mathf.Max(currentLineLimit, 1f);
    }

    public Vector3 GetHookPosition() => currentHook ? currentHook.transform.position : Vector3.zero;
}