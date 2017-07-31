using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "PowerCommand/AudioSettings", order = 2)]
public class AudioSettings : ScriptableObject {

    [SerializeField, Range(0.0f, 1.0f)]
    private float masterVolume = 0.2f, musicVolume = 0.2f;

    public float MasterVolume
    {
        get { return masterVolume; }
        set
        {
            if (masterVolume != value)
                masterVolume = Mathf.Clamp01(value);
        }
    }

    public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            if (musicVolume != value)
                musicVolume = Mathf.Clamp01(value);
        }
    }

}
