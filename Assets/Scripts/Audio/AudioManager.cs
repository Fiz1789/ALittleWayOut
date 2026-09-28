using UnityEngine;

namespace Platformer
{
    public class AudioManager : MonoBehaviour

    {
        [Header("Footstep SFX")]
        public AudioClip[] footstepClips; // Assign multiple for randomization, or just one
        [Range(0f, 1f)]

        public static AudioManager Instance;

        [Header("Background Music")]
        public AudioClip backgroundMusic;
        [Range(0f, 1f)] public float backgroundMusicVolume = 0.3f;
        // public AudioClip typingNormalLetterSoundEffect;

        public AudioClip tutorialMusic;
        [Range(0f, 1f)] public float tutorialMusicVolume = 0.2f;

        public AudioClip puzzleMusic;
        [Range(0f, 1f)] public float puzzleMusicVolume = 0.05f;

        [Header("Breaking Floor SFX")]
        public AudioClip breakingFloorClip;
        [Range(0f, 1f)] public float breakingFloorVolume = 0.7f;

        [Header("Spawn Obby SFX")]
        public AudioClip spawnObbyClip;

        public float spawnObbyVolume = 0.7f;

        [Header("Push/Pull SFX")]
        public AudioClip pushClip;
        public AudioClip pullClip;
        [Range(0f, 1f)] public float pushPullVolume = 0.7f;

        private AudioSource musicSource;
        private AudioSource sfxSource;
        // Cooldown for push/pull SFX
        [SerializeField] public float pushPullCooldown = 0.5f;
        private float lastPushTime = -10f;
        private float lastPullTime = -10f;

        public float footstepVolume = 0.6f;



        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // Create audio sources
            musicSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            // Configure music source
            musicSource.loop = true;
            musicSource.volume = backgroundMusicVolume; // Adjust volume as needed

            // Configure SFX source
            sfxSource.loop = false;
            sfxSource.volume = 0.5f;

            // if (backgroundMusic != null)
            // {
            //     musicSource.clip = backgroundMusic;
            //     musicSource.Play();
            // }

        }
        public void PlayMusic(AudioClip clip)
        {
            if (musicSource != null && clip != null)
            {
                musicSource.Stop();
                musicSource.clip = clip;
                musicSource.Play();
            }
        }
        public void PlayFootstep()
        {
            if (footstepClips != null && footstepClips.Length > 0 && sfxSource != null)
            {
                int idx = Random.Range(0, footstepClips.Length);
                sfxSource.PlayOneShot(footstepClips[idx], footstepVolume);
            }
        }
        public void StopMusic()
        {
            musicSource.Stop();
            sfxSource.Stop();


        }

        public void PlayPush()
        {
            if (pushClip != null && sfxSource != null)
            {
                if (Time.time - lastPushTime >= pushPullCooldown)
                {
                    sfxSource.PlayOneShot(pushClip, pushPullVolume);
                    lastPushTime = Time.time;
                }
            }
        }
        public void PlayPull()
        {
            if (pullClip != null && sfxSource != null)
            {
                if (Time.time - lastPullTime >= pushPullCooldown)
                {
                    sfxSource.PlayOneShot(pullClip, pushPullVolume);
                    lastPullTime = Time.time;
                }
            }
        }

        public void PlayBreakingFloor()
        {
            if (breakingFloorClip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(breakingFloorClip, breakingFloorVolume);
            }
        }
        public void PlaySpawnObby()
        {
            if (spawnObbyClip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(spawnObbyClip, spawnObbyVolume);
            }
        }
        public void SetMusicVolume(float value)
        {
            backgroundMusicVolume = value;
            tutorialMusicVolume = value;
            puzzleMusicVolume = value;
            // Remap slider: min = 0.0001, max = 0.3 (not full volume)
            float minVol = 0.0001f;
            float maxVol = 0.3f;
            float mappedValue = Mathf.Lerp(minVol, maxVol, value);
            // Logarithmic scaling for natural feel
            float volume = Mathf.Log10(Mathf.Clamp(mappedValue, 0.0001f, 1f)) * 20f;
            float linearVolume = Mathf.Pow(10f, volume / 20f);
            if (musicSource != null)
            {

                musicSource.volume = linearVolume;
            }
        }
    }

}