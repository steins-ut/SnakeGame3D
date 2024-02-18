using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//there is like 2 hours left i CANNOT be bothered with writing
//actually sane code

public class SoundManager : MonoBehaviour
{
    public static SoundManager s_Instance = null;

    private List<AudioSource> m_sources = new List<AudioSource>();
    private Queue<int> m_availableIndices = new Queue<int>();
    private Dictionary<int, int> m_songToSource = new Dictionary<int, int>();

    [SerializeField]
    private List<AudioClip> m_sounds;

    private void Awake()
    {
        s_Instance = this;
    }

    private void CreateAudioObject()
    {
        GameObject obj = new GameObject("Audio Player");

        obj.transform.parent = transform;
        m_availableIndices.Enqueue(m_sources.Count);
        m_sources.Add(obj.AddComponent<AudioSource>());
    }

    private IEnumerator HandleOnceFree(int id, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        m_availableIndices.Enqueue(id);

        yield break;
    }

    private int GetAudioSourceId()
    {
        if (m_availableIndices.Count == 0)
        {
            CreateAudioObject();
        }
        return m_availableIndices.Dequeue();
    }

    public void PlayBandageUse()
    {
        int id = GetAudioSourceId();
        m_sources[id].PlayOneShot(m_sounds[0]);
        StartCoroutine(HandleOnceFree(id, m_sounds[0].length));
    }

    public void PlayStabSound()
    {
        int id = GetAudioSourceId();
        m_sources[id].PlayOneShot(m_sounds[4]);
        StartCoroutine(HandleOnceFree(id, m_sounds[4].length));
    }

    public void PlayButtonSound()
    {
        int id = GetAudioSourceId();
        m_sources[id].PlayOneShot(m_sounds[1]);
        StartCoroutine(HandleOnceFree(id, m_sounds[1].length));
    }

    public void PlayVialPutSound()
    {
        int id = GetAudioSourceId();
        m_sources[id].PlayOneShot(m_sounds[6]);
        StartCoroutine(HandleOnceFree(id, m_sounds[6].length));
    }

    public void PlayKnifePutSound()
    {
        int id = GetAudioSourceId();
        m_sources[id].PlayOneShot(m_sounds[8]);
        StartCoroutine(HandleOnceFree(id, m_sounds[8].length));
    }

    public void PlayMicrowaveHum()
    {
        if (m_songToSource.ContainsKey(5)) return;

        int id = GetAudioSourceId();
        m_sources[id].loop = true;
        m_sources[id].clip = m_sounds[5];
        m_sources[id].Play();
        m_songToSource.Add(5, id);
    }

    public void StopMicrowaveHum()
    {
        if (!m_songToSource.ContainsKey(5)) return;

        int id = m_songToSource[5];
        m_sources[id].loop = false;
        m_sources[id].Stop();
        m_availableIndices.Enqueue(id);
        m_songToSource.Remove(5);
    }

    public void PlaySlowBreath()
    {
        if (m_songToSource.ContainsKey(7)) return;

        int id = GetAudioSourceId();
        m_sources[id].loop = true;
        m_sources[id].clip = m_sounds[7];
        m_sources[id].Play();
        m_songToSource.Add(7, id);
    }

    public void StopSlowBreath()
    {
        if (!m_songToSource.ContainsKey(7)) return;

        int id = m_songToSource[7];
        m_sources[id].loop = false;
        m_sources[id].Stop();
        m_availableIndices.Enqueue(id);
        m_songToSource.Remove(7);
    }

    public void PlayPanSound()
    {
        int id = GetAudioSourceId();
        m_sources[id].PlayOneShot(m_sounds[9]);
        StartCoroutine(HandleOnceFree(id, m_sounds[9].length));
    }
}