using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(MissileSpawner), typeof(AudioHelper))]
public class GameController : MonoBehaviour {

    public event EventHandler<EventArgs> OnPreparationEnded;
    public event EventHandler<EventArgs> OnAttackEnded;
    public event EventHandler<EventArgs> OnGameLost;

    [SerializeField, Range(1.0f, 100.0f)]
    private float powerPerRound = 20.0f;
    [SerializeField]
    private List<PlayerBase> playerBases = new List<PlayerBase>();
    [SerializeField]
    private float currentPower = 0.0f;

    private MissileSpawner spawner = null;
    private bool preparationPhase = true, fastForward = false;

    public List<PlayerBase> PlayerBases { get { return playerBases; } }
    public float CurrentPower { get { return currentPower; } }
    public bool HasLost
    {
        get { return CurrentPower < 0.0f || PlayerBases.Where(_base => _base.IsIntact).Count() == 0; }
    }

    private void Start()
    {
        if(!gameObject.TryGetComponent(out spawner))
        {
            Debug.LogError("No Missilespawner attached!");
        }

        spawner.OnAllMissilesDetonated += OnAllMissilesDetonated;
    }

    private void OnDestroy()
    {
        spawner.OnAllMissilesDetonated -= OnAllMissilesDetonated;
    }

    /// <summary>
    /// Adds power to the player
    /// </summary>
    public void AddPower(float power)
    {
        currentPower += power;
        if (currentPower < 0.0f && OnGameLost != null)
            OnGameLost.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Sets whether the attack-phase should be fast-forwarded or not
    /// </summary>
    public void ToggleFastForward()
    {
        fastForward = !fastForward;
        UpdateTimeScale();
    }

    /// <summary>
    /// Starts the next game-phase
    /// </summary>
    public void NextRound()
    {
        IsPreparationPhase = !IsPreparationPhase;
        if(!IsPreparationPhase)
        {
            spawner.LaunchWave();
        }
        UpdateTimeScale();
    }

    /// <summary>
    /// Gets or sets if the player can prepare for the next attack of if the next attack was already started
    /// </summary>
    public bool IsPreparationPhase
    {
        get { return preparationPhase; }
        set
        {
            if(value != preparationPhase)
            {
                preparationPhase = value;

                if (preparationPhase && OnAttackEnded != null)
                    OnAttackEnded.Invoke(this, new EventArgs());
                else if (!preparationPhase && OnPreparationEnded != null)
                    OnPreparationEnded.Invoke(this, new EventArgs());
            }
        }
    }

    /// <summary>
    /// Restarts the level
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Updates the time-scale to the required speed
    /// </summary>
    private void UpdateTimeScale()
    {
        if (!IsPreparationPhase && fastForward)
        {
            Time.timeScale = 5;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void OnAllMissilesDetonated(object sender, EventArgs e)
    {
        if (!HasLost)
        {
            NextRound();
            currentPower += powerPerRound;
        }
        else if(OnGameLost != null)
        {
            OnGameLost.Invoke(this, new EventArgs());
        }
    }
}
