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

    private bool collisionsAreEnabled = true;
    
    [SerializeField] private float thrust = 80000f;
    [SerializeField] private float rcsThrust = 150f;
    [SerializeField] private float levelLoadDelay = 1f;
    
    [SerializeField] private AudioClip mainEngine;
    [SerializeField] private AudioClip succeedLevel;
    [SerializeField] private AudioClip youDied;

    [SerializeField] private ParticleSystem engine;
    [SerializeField] private ParticleSystem success;
    [SerializeField] private ParticleSystem death;

    private bool isTransitioning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug.isDebugBuild)
        {
            checkDebugKeys();
        }
        
        if (!isTransitioning)
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
        if (isTransitioning || !collisionsAreEnabled)
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

    void checkDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
            print("Loading next level.");

        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            // toggle collisions
            collisionsAreEnabled = !collisionsAreEnabled;
            print("Toggling collisions: " + collisionsAreEnabled);

        }
    }

    private void LevelFinish()
    {
        print("Finished Level");
        isTransitioning = true;
        engineSound.Stop();
        engineSound.PlayOneShot(succeedLevel);
        success.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void DeathSequence()
    {
        print("Dead.");
        isTransitioning = true;
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

    private void LoadNextScene() 
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if(currentSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0); //loop back to start
        }
        else
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
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
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        engineSound.Stop();
        engine.Stop();
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
        rigidBody.angularVelocity = Vector3.zero; //remove rotation due to physics
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            //print("Rotating Left");
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D) && !isTransitioning)
        {
            //print("Rotating Right");
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

    }

}
