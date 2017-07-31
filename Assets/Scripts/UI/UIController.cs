using UnityEngine.UI;
using UnityEngine;
using System;

[RequireComponent(typeof(GameController))]
public class UIController : MonoBehaviour {

    [SerializeField]
    private Transform ingameMenu = null;
    [SerializeField, Range(0.1f, 10.0f)]
    private float ingameMenuSmoothTime = 1.0f;
    [SerializeField]
    private Button gamePhaseButton = null;
    [SerializeField]
    private Text currentPowerText = null, gamePhaseText = null, gameLostText = null;

    private GameController controller = null;
    private Vector3 origMenuPos = Vector3.zero, targetMenuPos = Vector3.zero;
    private Coroutine menuFadeRoutine = null;
    private bool fadeIn = true;

    private void Start () {
        if(!gameObject.TryGetComponent(out controller))
        {
            Debug.LogError("No GameController attached!");
        }
        controller.OnAttackEnded += OnGamePhaseChanged;
        controller.OnPreparationEnded += OnGamePhaseChanged;
        controller.OnGameLost += OnGameLost;

        origMenuPos = ingameMenu.position;
        targetMenuPos = ingameMenu.position + Vector3.right * Screen.width;
        ingameMenu.position = targetMenuPos;

        UpdateGamePhaseUI();
        UpdatePowerCounter();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            FadeIngameMenu();
        }
    }

    private void OnDestroy()
    {
        controller.OnAttackEnded -= OnGamePhaseChanged;
        controller.OnPreparationEnded -= OnGamePhaseChanged;
        controller.OnGameLost -= OnGameLost;
    }

    public void UpdateGamePhaseUI()
    {
        gamePhaseText.text = "Phase: " + (controller.IsPreparationPhase ? "Preparation" : "Attack");
        gamePhaseButton.interactable = controller.IsPreparationPhase;
    }

    public void UpdatePowerCounter()
    {
        float power = Mathf.Floor(controller.CurrentPower);
        float tmp = power;
        int unit = 0;
        
        while(tmp / 1000 >= 1)
        {
            power = tmp /= 1000;
            unit++;
        }

        currentPowerText.text = Math.Round(tmp, 1) + " " + GetUnit(unit);
    }

    private void OnGamePhaseChanged(object sender, EventArgs e)
    {
        UpdateGamePhaseUI();
        UpdatePowerCounter();
    }

    private void OnGameLost(object sender, EventArgs e)
    {
        gamePhaseButton.interactable = false;
        gameLostText.text = "You Lost\n(Restart or exit)";

        fadeIn = true;
        FadeIngameMenu();
    }

    private void FadeIngameMenu()
    {
        fadeIn = !fadeIn;
        if (menuFadeRoutine != null)
            StopCoroutine(menuFadeRoutine);
        menuFadeRoutine = StartCoroutine(GameTools.Fade(ingameMenu, fadeIn ? targetMenuPos : origMenuPos, ingameMenuSmoothTime));
    }

    private string GetUnit(int index)
    {
        string output = String.Empty;
        switch(index)
        {
            case 1:
                output = "K";
                break;
            case 2:
                output = "M";
                break;
            case 3:
                output = "G";
                break;
            case 4:
                output = "T";
                break;
            case 5:
                output = "P";
                break;
            case 6:
                output = "E";
                break;
            case 7:
                output = "Z";
                break;
            case 8:
                output = "Y";
                break;
            default:
                output = String.Empty;
                break;
        }
        return output;
    }
}
