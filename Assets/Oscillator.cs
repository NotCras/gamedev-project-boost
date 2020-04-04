using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] private Vector3 movementVector = new Vector3(10f, 0, 0);
    [SerializeField] private float period = 2f;
    
    [SerializeField] [Range(0, 1)] private float movementFactor;

    private Vector3 startingPose;
    
    // Start is called before the first frame update
    void Start()
    {
        startingPose = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon)
        {
            movementFactor = 0;
        }
        else
        {
            float cycles = Time.time / period;
            const float tau = Mathf.PI * 2;

            float rawSineWave = Mathf.Sin(tau * cycles);

            movementFactor = rawSineWave / 2f + 0.5f;
        }

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPose + offset;
    }
}
