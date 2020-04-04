using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class Rocket : MonoBehaviour
{
    private Rigidbody rigidBody;
    private AudioSource engineSound;
    
    [SerializeField] private float thrust = 80000f;
    [SerializeField] private float rcsThrust = 150f;
    [SerializeField] private float levelLoadDelay = 1f;
    
    [SerializeField] private AudioClip mainEngine;
    [SerializeField] private AudioClip succeedLevel;
    [SerializeField] private AudioClip youDied;

    [SerializeField] private ParticleSystem engine;
    [SerializeField] private ParticleSystem success;
    [SerializeField] private ParticleSystem death;
    
    enum State
    {
        Alive,
        Dying,
        Transcending
    };

    private State state = State.Alive;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        else
        {
            // do nothing
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return; //don't do anything else
        }
        
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                LevelFinish();
                break;
/*            case "Wall":
                break;*/
            default:
                DeathSequence();
                break;
        }
    }

    private void LevelFinish()
    {
        print("Finished Level");
        state = State.Transcending;
        engineSound.Stop();
        engineSound.PlayOneShot(succeedLevel);
        success.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void DeathSequence()
    {
        print("Dead.");
        state = State.Dying;
        engineSound.Stop();
        engineSound.PlayOneShot(youDied);
        
        death.Play();
        engine.Stop();
        
        //rigidBody.AddRelativeTorque(Random.rotation.eulerAngles * 50000);
        rigidBody.AddRelativeTorque(Vector3.up * 500000);
        Invoke("Dying", levelLoadDelay);
    }

    private void Dying()
    {
        rigidBody.freezeRotation = true;
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene() //TODO more than two levels
    {
        SceneManager.LoadScene(1);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.S))
        {
            //print("Thrusting");

            ApplyThrust();
        }
        else
        {
            engineSound.Stop();
            engine.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = thrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

        if (!engineSound.isPlaying)
        {
            engineSound.PlayOneShot(mainEngine);
        }

        engine.Play();
    }


    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            //print("Rotating Left");
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D) && state == State.Alive)
        {
            //print("Rotating Right");
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // give control of rotation back to physics
    }

}
