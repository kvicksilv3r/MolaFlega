using UnityEngine;
using UnityEngine.UI;

public class ContinentToggleButton : MonoBehaviour
{
    private bool continentActive = false;
    public GameManager gameManager;

    public Continent continent;
    public Outline outline;

    void Start()
    {
        continentActive = gameManager.ContainsContinent(continent);
        SetVisuals();
    }

    public void OnButtonPressed()
    {
        continentActive = gameManager.ToggleContinent(continent);
        SetVisuals();
    }

    public void SetVisuals()
    {
        outline.enabled = continentActive;
    }
}
