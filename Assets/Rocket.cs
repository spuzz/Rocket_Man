using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1000f;

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
            Thrust();
            Rotate();
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                if(currentScene + 1 < SceneManager.sceneCountInBuildSettings)
                {
                    state = State.Transcending;
                    Invoke("LoadNextLevel", 1f);

                }
                
                break;
            default:
                state = State.Dying;
                Invoke("LoadCurrentLevel", 1f);
                break;
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

    private void Rotate()
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

    private void Thrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
}
