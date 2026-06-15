using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip[] footstepClips;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    
    [Header("Loudness")]
    public float footstepLoudnessWalk = 0.7f;
    public float footstepLoudnessRun = 1.2f;
    
    private AudioSource audioSource;
    private float stepTimer = 0f;
    private int lastPlayedIndex = -1;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0.6f;
    }
    
    void Update()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || 
                        Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        
        if (isMoving)
        {
            stepTimer += Time.deltaTime;
            
            float currentInterval = walkStepInterval;
            float currentLoudness = footstepLoudnessWalk;
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentInterval = runStepInterval;
                currentLoudness = footstepLoudnessRun;
            }
            
            if (stepTimer >= currentInterval)
            {
                PlayFootstepSound(currentLoudness);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
    
    void PlayFootstepSound(float loudness)
    {
        if (footstepClips == null || footstepClips.Length == 0)
            return;
        
        int randomIndex = Random.Range(0, footstepClips.Length);
        
        while (footstepClips.Length > 1 && randomIndex == lastPlayedIndex)
        {
            randomIndex = Random.Range(0, footstepClips.Length);
        }
        lastPlayedIndex = randomIndex;
        
        if (footstepClips[randomIndex] != null)
        {
            audioSource.PlayOneShot(footstepClips[randomIndex], 0.6f);
            NotifyMonsters(loudness);
        }
    }
    
    void NotifyMonsters(float loudness)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            MonsterAI monsterAI = monster.GetComponent<MonsterAI>();
            if (monsterAI != null)
            {
                monsterAI.HearSound(transform.position, loudness);
            }
        }
    }
}