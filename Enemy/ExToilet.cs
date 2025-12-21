using JYW.Game.Commons;
using UnityEngine;

namespace JYW.Game.Enemies
{
    public class ExToilet : MonoBehaviour, IEnemy
    {
        [SerializeField] private GameObject destroyParticle;
        private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void GetHit()
        {
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            audioSource.PlayOneShot(audioClip, 0.3f);
            Instantiate(destroyParticle, gameObject.transform.position + gameObject.transform.up * 5f, Quaternion.Euler(0, 0, 0));
        }



    }
}