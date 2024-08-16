using UnityEngine;

namespace BigRookGames.Build
{
    public class BasicWoodWallController : MonoBehaviour
    {
        
        public Animator anim;
        [SerializeField] public int m_Health = 20;

        public GameObject wallExplosionPrefab;
        public Transform wallExplosionPosition;
        public bool alive = false;
        public AudioClip spawnClip, damageClip, deathClip;
        public AudioSource damageAudioSource;

        // --- Variable to determine when to play damage animations and audio ---
        // --- Stage 0 - health: Full Health
        // --- Stage 1 - health: 71-99
        // --- Stage 2 - health: 21-70
        // --- Stage 3 - health: 1-20
        // --- Stage 4 - health: 0;
        public int panelStage = 0;



        private void Awake()
        {
            anim = GetComponent<Animator>();
            damageAudioSource = GetComponent<AudioSource>();
            if (!alive) gameObject.SetActive(false);
        }

        void Start()
        {
            damageAudioSource.clip = spawnClip;
            damageAudioSource.Play();
        }


        private void Update()
        {
            // --- If alive and the health has crossed below the next threshold, update panel ---
            if (alive && CheckStageHealthThreshold())
            {
                anim.SetInteger("Health", m_Health);
                if (m_Health <= 0)
                {
                    alive = false;
                    damageAudioSource.clip = deathClip;
                    damageAudioSource.Play();
                    Instantiate(wallExplosionPrefab, wallExplosionPosition);
                }
                else
                {
                    damageAudioSource.clip = spawnClip;
                    damageAudioSource.Play();
                }
            }

            // --- For Example Scene, Take Out In Real Project (set health to 100 to reset wall) ---
            else if (!alive)
            {
                if (m_Health == 20)
                {
                    alive = true;
                }
            }
        }

        private bool CheckStageHealthThreshold()
        {
            switch(panelStage)
            {
                case 0:
                    if (m_Health < 20)
                    {
                        panelStage++;
                        return true;
                    }
                    break;
                case 1:
                    if (m_Health < 15)
                    {
                        panelStage++;
                        return true;
                    }
                    break;
                case 2:
                    if (m_Health < 10)
                    {
                        panelStage++;
                        return true;
                    }
                    break;
                case 3:
                    if (m_Health <= 0)
                    {
                        panelStage++;
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Sets the Health integer parameter on the animator to play each animation.
        /// This is called from the UI buttons in the example scene.
        /// </summary>
        /// <param name="damageStage"></param>
        public void PlayDamageAnimation(int damageStage)
        {
            switch (damageStage)
            {
                case 0:
                    m_Health = 20;
                    break;
                case 1:
                    m_Health = 15;
                    break;
                case 2:
                    m_Health = 10;
                    break;
                case 3:
                    m_Health = 5;
                    break;
                case 4:
                    m_Health = 0;
                    break;
            }
            anim.SetInteger("Health", m_Health);
        }
    }
}