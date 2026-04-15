using System.Collections.Generic;
using UnityEngine;

public class FishingRodCasting : MonoBehaviour
{
    public Transform rodTip;
    public GameObject hookPrefab;
    public LineRenderer line;
    public List<FishData> fishList;
    private FishData currentFishData;
    public float castForce = 20f;
    public float reelSpeed = 5f;
    public float maxLineLength = 15f;
    public float slackRecoverSpeed = 3f;
    
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
            Vector3 random = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0,
                Random.Range(-0.5f, 0.5f)
            );

            hookRb.AddForce(random * 5f, ForceMode.Force);
        }
    }


    public FishData SpawnFish()
    {
        if (fishList.Count == 0) return null;

        int index = Random.Range(0, fishList.Count);
        currentFishData = fishList[index];
        if (currentFishData.prefab == null) return null;
        currentFish = Instantiate(currentFishData.prefab);
        currentFish.transform.position = currentHook.transform.position;
        currentFish.transform.SetParent(currentHook.transform);
        Rigidbody rb = currentFish.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        return currentFishData;
    }

    public void ResetCast()
    {
        isCast = false;
        if (currentHook)
        {
            Destroy(currentHook);
        }

        if (currentFish)
        {
            //Rigidbody rb = currentFish.GetComponent<Rigidbody>();
            //if (rb != null)
            //    rb.isKinematic = false;

            //currentFish.transform.SetParent(null);
            //currentFish = null;

            Destroy(currentFish);
        }

        line.positionCount = 0;
        currentFishData = null;
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

    public void RelaxLine()
    {
        currentLineLimit += slackRecoverSpeed * Time.deltaTime;
        currentLineLimit = Mathf.Min(currentLineLimit, maxLineLength);
    }

    public Vector3 GetHookPosition() => currentHook ? currentHook.transform.position : Vector3.zero;
    
    public FishData GetCurrentFishData()
    {
        return currentFishData;
    }
}