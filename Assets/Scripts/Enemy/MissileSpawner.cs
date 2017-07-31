using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(GameController))]
public class MissileSpawner : MonoBehaviour {

    public event EventHandler<EventArgs> OnAllMissilesDetonated;

    [SerializeField]
    private List<GameObject> missilePrefabs = new List<GameObject>();
    [SerializeField, Range(0.0f, 0.49f)]
    private float minLaunchOffset = 0.25f;
    [SerializeField, Range(0.5f, 1.0f)]
    private float maxLaunchOffset = 0.25f;
    [SerializeField]
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField, Range(1.2f, 2.0f)]
    private float waveStrengthFactor = 1.5f;

    private List<GameObject> missiles = new List<GameObject>();
    private Coroutine missileLaunchRoutine = null;
    private int nextWaveCount = 5;
    private GameController controller = null;

    private void Start()
    {
        if(!gameObject.TryGetComponent(out controller))
        {
            Debug.LogError("No GameController attached!");
        }
        targets = controller.PlayerBases.Select(_base => _base.gameObject).ToList();
    }

    private void OnDestroy()
    {
        BaseMissile missile = null;
        foreach(GameObject obj in missiles)
        {
            if (obj.TryGetComponent(out missile))
                missile.OnExploded -= OnMissileExploded;
        }
    }

    /// <summary>
    /// Orders a new wave of missiles to be launched
    /// </summary>
    public void LaunchWave()
    {
        missileLaunchRoutine = StartCoroutine(LaunchMissiles());
    }

    /// <summary>
    /// Launches the wave of missiles
    /// </summary>
    private IEnumerator LaunchMissiles()
    {
        for (int i = 0; i < nextWaveCount; i++)
        {
            missiles.Add(Instantiate(missilePrefabs.GetRandom(), transform.position + transform.right * UnityEngine.Random.Range(-5.0f, 5.0f), Quaternion.identity, transform));

            BaseMissile missile = null;
            if (targets.Count > 0 && missiles.Last().TryGetComponent(out missile))
            {
                missile.Target = targets.GetRandom();
                missile.OnExploded += OnMissileExploded;
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(minLaunchOffset, maxLaunchOffset));
        }
    }

    private void OnMissileExploded(object sender, MissileEventArgs e)
    {
        missiles.Remove(e.Missile.gameObject);
        e.Missile.OnExploded -= OnMissileExploded;

        if (missiles.Count == 0)
        {
            nextWaveCount = (int)(nextWaveCount * waveStrengthFactor);
            if (OnAllMissilesDetonated != null)
                OnAllMissilesDetonated.Invoke(this, new EventArgs());
        }
    }
}
