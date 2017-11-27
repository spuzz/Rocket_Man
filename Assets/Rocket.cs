using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip audioDying;
    [SerializeField] AudioClip audioFinished;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    enum State { Alive, Dying, Transcending }
    [SerializeField]  State state = State.Alive;
    int currentScene;
    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        currentScene = SceneManager.GetActiveScene().buildIndex;
        int test = SceneManager.sceneCountInBuildSettings;
    }
	
	// Update is called once per frame
	void Update () {
        if(state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audioDying);
        state = State.Dying;
        deathParticles.Play();
        Invoke("LoadCurrentLevel", 1f);
    }

    private void StartSuccessSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audioFinished);
        successParticles.Play();
        if (currentScene + 1 < SceneManager.sceneCountInBuildSettings)
        {
            state = State.Transcending;
            Invoke("LoadNextLevel", 1f);
        }
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(currentScene + 1);
    }

    private void LoadCurrentLevel()
    {
        SceneManager.LoadScene(currentScene);
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;

       
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }

    private void RespondToThrustInput()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust(thrustThisFrame);
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust(float thrustThisFrame)
    {
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
