using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [SerializeField] private List<Floaters> floaters = new List<Floaters>();
    [SerializeField] private float waterLine = 0f;
    [SerializeField] private float underWaterDrag = 3f;
    [SerializeField] private float underWaterAngularDrag = 1f;
    [SerializeField] private float defaultDrag = 0f;
    [SerializeField] private float defaultAngularDrag = 0.05f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool isUnderWater = false;

        for(int i = 0; i < floaters.Count; i++)
        {
           if (floaters[i].FloaterUpdate(rb,waterLine))
           {
               isUnderWater = true;
            }
        }

        SetState(isUnderWater);
    }

    private void SetState(bool isUnderWater)
    {
        if (isUnderWater)
        {
            rb.linearDamping = underWaterDrag;
            rb.angularDamping = underWaterAngularDrag;

        }
        else
        {
            rb.linearDamping = defaultDrag;
            rb.angularDamping = defaultAngularDrag;
        }
    }
}

[System.Serializable]
public class Floaters
{
    [SerializeField] private float floatingPower = 80f;
    [SerializeField] private Transform floater;

    private bool underwater;

    public bool FloaterUpdate(Rigidbody rb, float waterLine)
    {
        float distance = floater.position.y - waterLine;

        if (distance < 0f)
        {
            rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(distance), floater.position, ForceMode.Force);
            if (!underwater)
            {
                underwater = true;
            }
        }
        else if (underwater)
        {
            underwater = false;
        }

        return underwater;
    }
}
