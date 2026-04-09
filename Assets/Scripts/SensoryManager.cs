using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryManager : MonoBehaviour
{
    public GameObject soundObjects;
    public StateMachine npc;

    private void Awake()
    {
        foreach (Transform transform in soundObjects.transform)
        {
            SoundObject soundObject = transform.GetComponent<SoundObject>();
            if (soundObject != null)
            {
                soundObject.OnSoundTriggered.AddListener(npc.SoundTrigger);
            }
        }
    }
}
