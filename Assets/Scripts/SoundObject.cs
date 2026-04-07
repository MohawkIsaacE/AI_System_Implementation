using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class SoundObject : MonoBehaviour
{
    public UnityEvent<SoundObject> OnSoundTriggered = new UnityEvent<SoundObject>();

    AudioSource source;

    public virtual void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        // Activated when the player or an NPC steps on it
        if (other.GetComponent<PlayerController>() != null || other.GetComponent<StateMachine>() != null)
        {
            source.Play();
            OnSoundTriggered.Invoke(this);
        }
    }
}
