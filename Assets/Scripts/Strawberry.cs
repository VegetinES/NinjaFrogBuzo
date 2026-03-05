using UnityEngine;

public class Strawberry : MonoBehaviour
{
    [HideInInspector] public int strawberryId;
    [HideInInspector] public StrawberrySpawner spawner;

    public void SetupAudio(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("🍓 AudioClip es null en fresa " + strawberryId);
            return;
        }

        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = 1f;
        src.maxDistance = 15f;
        src.volume = 0.7f;
        src.Play();
        Debug.Log("🍓 Audio iniciado en fresa " + strawberryId);
    }
}
