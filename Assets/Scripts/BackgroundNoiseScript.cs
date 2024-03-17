using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundNoiseScript : MonoBehaviour
{
    private float timer = 0;
    public float noiseRateEarliest = 30;
    public float noiseRateLatest = 60;
    private float nextNoiseTimer;
    private AudioSource clankNoise;
    public AudioClip[] clankNoises;


    private void Awake()
    {
        clankNoise = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nextNoiseTimer = Random.Range(noiseRateEarliest, noiseRateLatest);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < nextNoiseTimer)
            timer += Time.deltaTime;
        else
        {
            Debug.Log("Playing clank sound.");
            clankNoise.clip = clankNoises[Random.Range(0, clankNoises.Length)];
            clankNoise.PlayOneShot(clankNoise.clip);
            timer = 0;
            nextNoiseTimer = Random.Range(noiseRateEarliest, noiseRateLatest);
        }
    }
}
