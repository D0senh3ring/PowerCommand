using UnityEngine.UI;
using UnityEngine;

public class UITooltip : MonoBehaviour {

    [SerializeField]
    private Vector3 displayOffset = Vector3.zero;
    
    private Text displayText = null;

    private void Start()
    {
        if (!gameObject.TryGetComponent(out displayText) && !gameObject.TryGetComponentInChildren(out displayText))
            Debug.LogError("No Text attached to GameObject!");

        Hide();
    }

    public void Display(string text)
    {
        displayText.text = text;
        //transform.position = Input.mousePosition + displayOffset;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
