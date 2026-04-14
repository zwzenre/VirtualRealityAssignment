using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodCasting : MonoBehaviour
{
    public Transform rodTip;        
    public GameObject hookPrefab;     
    public LineRenderer line;
    public GameObject fishPrefab;
    private GameObject currentFish;

    public float castForce = 20f;
    public int maxPoints = 50;
    public float maxLineLength = 10f;
    private float currentLineLength = 10f;
    public float reelSpeed = 5f;
    private bool justCast = false;

    private GameObject currentHook;
    private Rigidbody hookRb;

    private bool isCast = false;


    void Start()
    {
        line.positionCount = 0;
        if (currentHook != null)
        {
            hookRb = currentHook.GetComponent<Rigidbody>();
            hookRb.linearDamping = 2f;
        }
    }
    void Update()
    {
        if (hookRb != null)
        {
            hookRb.linearVelocity *= 0.98f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CastLine();
        }

        if (Input.GetKey(KeyCode.R))
        {
            currentLineLength -= reelSpeed * Time.deltaTime;
            currentLineLength = Mathf.Clamp(currentLineLength, 2f, 20f);
        }

        if (isCast && currentHook != null)
        {
            UpdateLine();
            LimitLineLength();

        }
    }
    public bool IsCast()
    {
        return isCast;
    }
    public float GetLineLength()
    {
        return currentLineLength;
    }

    public Vector3 GetHookPosition()
    {
        if (currentHook != null)
            return currentHook.transform.position;

        return Vector3.zero;
    }

    public bool JustCast()
    {
        if (justCast)
        {
            justCast = false;
            return true;
        }
        return false;
    }

    void CastLine()
    {
        currentLineLength = maxLineLength;
        if (currentHook != null)
        {
            Destroy(currentHook);
        }


        currentHook = Instantiate(hookPrefab, rodTip.position, rodTip.rotation);
        hookRb = currentHook.GetComponent<Rigidbody>();

        hookRb.AddForce(rodTip.forward * castForce, ForceMode.Impulse);

        isCast = true;

        line.positionCount = 2;

        justCast = true;
    }
    void UpdateLine()
    {
        if (currentHook == null) return;

        int segmentCount = 20;
        line.positionCount = segmentCount;

        Vector3 start = rodTip.position;
        Vector3 end = currentHook.transform.position;

        float distance = Vector3.Distance(start, end);

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);

            Vector3 point = Vector3.Lerp(start, end, t);

            float sagFactor = Mathf.Sin(t * Mathf.PI);

            sagFactor *= (1 - t);

            float tension = Mathf.Clamp01(distance / currentLineLength);
            float sagAmount = Mathf.Lerp(0.2f, 0.02f, tension);
            float sag = sagFactor * distance * sagAmount;

            point += Vector3.down * sag;

            line.SetPosition(i, point);
        }
    }

    void LimitLineLength()
    {
        if (currentHook == null) return;

        Vector3 rodPos = rodTip.position;
        Vector3 hookPos = currentHook.transform.position;

        float distance = Vector3.Distance(rodPos, hookPos);

        if (distance >= currentLineLength)
        {
            Vector3 dir = (hookPos - rodPos).normalized;
            currentHook.transform.position = rodPos + dir * currentLineLength;

            hookRb.linearVelocity = Vector3.zero;
            hookRb.useGravity = false;
        }
        else
        {
            hookRb.useGravity = true;
        }
    }
    public void SimulateFish()
    {
        if (currentHook == null) return;

        Vector3 pullDir = new Vector3(
            Mathf.Sin(Time.time * 2f),
            -0.3f,
            Mathf.Cos(Time.time * 2f)
        );

        hookRb.AddForce(pullDir * 5f, ForceMode.Force);
    }

    public void SpawnFish()
    {
        if (currentHook == null) return;

        // 生成在 hook 附近
        currentFish = Instantiate(
            fishPrefab,
            currentHook.transform.position,
            Quaternion.identity
        );

        // 👉 直接绑在 hook 上（最简单）
        currentFish.transform.SetParent(currentHook.transform);

        // 调整一点位置（避免重叠）
        currentFish.transform.localPosition = new Vector3(0, -0.2f, 0);
    }
}