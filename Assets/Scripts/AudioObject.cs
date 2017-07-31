using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioObject : MonoBehaviour {

    protected AudioHelper helper = null;
    protected AudioSource source = null;

    protected virtual void Start()
    {
        if (!gameObject.TryGetComponent(out source))
            Debug.LogError("No AudioSource attached to Object!");

        GameObject controllerObject = GameObject.FindWithTag("GameController");
        if (!controllerObject.TryGetComponent(out helper))
            Debug.LogError("No AudioHelper attached to Object!");
        source.volume = helper.MasterVolume;

        helper.OnMasterVolumeChanged += OnMasterVolumeChanged;
    }

    protected virtual void OnDestroy()
    {
        helper.OnMasterVolumeChanged -= OnMasterVolumeChanged;
    }

    protected virtual void OnMasterVolumeChanged(object sender, EventArgs e)
    {
        source.volume = helper.MasterVolume;
    }
}
