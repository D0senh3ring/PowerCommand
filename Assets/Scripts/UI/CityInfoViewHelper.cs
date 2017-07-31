using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(GameController), typeof(UIController))]
public class CityInfoViewHelper : MonoBehaviour {

    [SerializeField]
    private Transform cityPanelObject = null;
    [SerializeField, Range(0.01f, 10.0f)]
    private float fadeSmoothTime = 1.0f;
    [SerializeField]
    private Image[] powerUpgradeIndicators = null;
    [SerializeField]
    private Image[] shieldUpgradeIndicators = null;
    [SerializeField]
    private Image[] repairUpgradeIndicators = null;
    [SerializeField]
    private Text producedPowerText = null;
    [SerializeField]
    private LayerMask cityLayer = 0, otherLayers = 0;
    [SerializeField]
    private Sprite[] powerSprites = null;
    [SerializeField]
    private Sprite[] shieldSprites = null;
    [SerializeField]
    private Sprite[] repairSprites = null;

    private PlayerBase selectedCity = null;
    private GameController controller = null;
    private UIController uiController = null;
    private RaycastHit2D hit;
    private Vector3 offsideCityPanelPos = Vector3.zero, originalCityPanelPos = Vector3.zero;
    private Coroutine fadeRoutine = null;

    public bool IsOverUI { get; set; }

    private void Start()
    {
        if (!gameObject.TryGetComponent(out controller))
            Debug.LogError("No GameController attached to GameObject!");

        if (!gameObject.TryGetComponent(out uiController))
            Debug.LogError("No UIController attached to GameObject!");

        originalCityPanelPos = cityPanelObject.position;
        offsideCityPanelPos = cityPanelObject.position + Vector3.left * Screen.width * cityPanelObject.localScale.x;
        cityPanelObject.position = offsideCityPanelPos;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if ( (hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, cityLayer))
               && hit.collider.gameObject.TryGetComponent(out selectedCity) && !IsOverUI)
            {
                UpdateCityPanel();

                if (fadeRoutine != null)
                    StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(GameTools.Fade(cityPanelObject, originalCityPanelPos, fadeSmoothTime));
            }
            else if(!IsOverUI)
            {
                if (fadeRoutine != null)
                    StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(GameTools.Fade(cityPanelObject, offsideCityPanelPos, fadeSmoothTime));
            }
        }
    }

    private void UpdateCityPanel()
    {
        if(selectedCity != null)
        {
            BaseUpgrade upgrade = null;

            if ((upgrade = selectedCity.Upgrades.Where(_up => _up.GetType().Equals(typeof(ShieldUpgrade))).FirstOrDefault()) != null)
            {
                for(int i = 0; i < shieldUpgradeIndicators.Length; i++)
                {
                    shieldUpgradeIndicators[i].sprite = shieldSprites[upgrade.Tier - 1 >= i ? 0 : 1];
                }
            }
            if ((upgrade = selectedCity.Upgrades.Where(_up => _up.GetType().Equals(typeof(GeneratorUpgrade))).FirstOrDefault()) != null)
            {
                for (int i = 0; i < powerUpgradeIndicators.Length; i++)
                {
                    powerUpgradeIndicators[i].sprite = powerSprites[upgrade.Tier - 1 >= i ? 0 : 1];
                }
            }
            if ((upgrade = selectedCity.Upgrades.Where(_up => _up.GetType().Equals(typeof(RepairTeamUpgrade))).FirstOrDefault()) != null)
            {
                for (int i = 0; i < repairUpgradeIndicators.Length; i++)
                {
                    repairUpgradeIndicators[i].sprite = repairSprites[upgrade.Tier - 1 >= i ? 0 : 1];
                }
            }

            producedPowerText.text = Math.Round(selectedCity.PowerProduction, 1).ToString();
            uiController.UpdatePowerCounter();
        }
    }

    public void UpgradeShield()
    {
        Upgrade(typeof(ShieldUpgrade));
    }

    public void UpgradeGenerator()
    {
        Upgrade(typeof(GeneratorUpgrade));
    }

    public void UpgradeRepair()
    {
        Upgrade(typeof(RepairTeamUpgrade));
    }

    private void Upgrade(Type upgradeType)
    {
        if (selectedCity != null && controller.IsPreparationPhase && selectedCity.IsIntact)
        {
            BaseUpgrade upgrade = null;
            if ((upgrade = selectedCity.Upgrades.Where(_up => _up.GetType().Equals(upgradeType)).FirstOrDefault()) != null
                && controller.CurrentPower - upgrade.UpgradeCost >= 0.0f)
            {
                upgrade.Upgrade();
                UpdateCityPanel();
            }
        }
    }
}
