using UnityEngine.UI;
using UnityEngine;
using System;

public class AudioHelper : MonoBehaviour {

    public event EventHandler<EventArgs> OnMasterVolumeChanged;

    [SerializeField]
    private AudioSource musicSource = null;
    [SerializeField]
    private AudioSettings settings = null;
    [SerializeField]
    private Slider masterSlider = null, musicSlider = null;

    private float masterVolume = 0.2f, musicVolume = 0.2f;

    /// <summary>
    /// Gets or sets the currently set volume for all AudioSources
    /// </summary>
    public float MasterVolume {
        get { return masterVolume; }
        set
        {
            if (masterVolume != value)
            {
                masterVolume = Mathf.Clamp01(value);
                if (OnMasterVolumeChanged != null)
                    OnMasterVolumeChanged.Invoke(this, new EventArgs());
            }
        }
    }

    /// <summary>
    /// Gets or sets the volume for the ingame music
    /// </summary>
    public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            if(musicVolume != value)
            {
                musicVolume = Mathf.Clamp01(value);
                OnMasterVolumeChange(this, new EventArgs());
            }
        }
    }

    private void Start()
    {
        OnMasterVolumeChanged += OnMasterVolumeChange;
        if (OnMasterVolumeChanged != null)
            OnMasterVolumeChanged.Invoke(this, new EventArgs());

        if(settings != null)
        {
            MasterVolume = masterSlider.value = settings.MasterVolume;
            MusicVolume = musicSlider.value = settings.MusicVolume;
        }
    }

    private void OnDestroy()
    {
        OnMasterVolumeChanged -= OnMasterVolumeChange;

        if (settings != null)
        {
            settings.MasterVolume = MasterVolume;
            settings.MusicVolume = MusicVolume;
        }
    }

    private void OnMasterVolumeChange(object sender, EventArgs e)
    {
        musicSource.volume = MusicVolume * masterVolume;
    }
}
