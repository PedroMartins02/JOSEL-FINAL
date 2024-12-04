using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundQueueManager : MonoBehaviour
{
    private Queue<AudioClip> soundQueue = new Queue<AudioClip>();
    private AudioSource audioSource;
    private bool isPlaying = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void QueueSound(AudioClip clip)
    {
        if ((soundQueue.Any() && soundQueue.Last() == clip) || soundQueue.Count() > 2) return;

        soundQueue.Enqueue(clip);

        if (!isPlaying)
        {
            PlayNextSound();
        }
    }

    private void PlayNextSound()
    {
        if (soundQueue.Count > 0)
        {
            AudioClip nextClip = soundQueue.Dequeue();
            audioSource.clip = nextClip;
            audioSource.Play();
            isPlaying = true;

            Invoke(nameof(PlayNextSound), nextClip.length);
        }
        else
        {
            isPlaying = false;
        }
    }
}
