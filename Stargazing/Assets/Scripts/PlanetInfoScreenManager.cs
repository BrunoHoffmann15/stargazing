using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInfoManager : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text description;
    public GameObject canvas;
    private CanvasGroup canvasGroup;
    public Image planetImage;
    public GameObject planetMenu;
    private CanvasGroup planetMenuGroup;

    // Start is called before the first frame update
    void Start()
    {
        SetUpCanvas();
        planetMenu.SetActive(false);

        if (PlanetInfoRepository.planetDescriptions == null) 
            PlanetInfoRepository.LoadPlanetInfo();
    }

    void SetUpCanvas()
    {
        canvasGroup = canvas.GetComponent<CanvasGroup>();

        canvas.SetActive(false);
        canvasGroup.enabled = false;
        canvasGroup.alpha = 0f;               // Torna invisível
        canvasGroup.interactable = false;     // Desativa interações
        canvasGroup.blocksRaycasts = false;   // Bloqueia cliques

        planetMenuGroup = planetMenu.GetComponent<CanvasGroup>();

        planetMenu.SetActive(false);
        planetMenuGroup.enabled = false;
        planetMenuGroup.alpha = 0f;               // Torna invisível
        planetMenuGroup.interactable = false;     // Desativa interações
        planetMenuGroup.blocksRaycasts = false;   // Bloqueia cliques
    }

    public void OpenAndClosePlanetsMenu()
    {
        planetMenu.SetActive(!planetMenu.activeSelf);

        if (planetMenu.activeSelf)
        {
            planetMenuGroup.alpha = 1f;
            planetMenuGroup.interactable = true;
            planetMenuGroup.blocksRaycasts = true;
            planetMenu.SetActive(true);
        }
        else
        {
            planetMenuGroup.enabled = false;
            planetMenuGroup.alpha = 0f;               // Torna invisível
            planetMenuGroup.interactable = false;     // Desativa interações
            planetMenuGroup.blocksRaycasts = false;   // Bloqueia cliques
        }
    }

    public void LoadPlanetInformation(Object buttonObject)
    {
        string planetName = buttonObject.name;
        LoadPlanetInfoToCanvas(planetName);
    }

    public void LoadImage(string planetName)
    {
        Sprite sprite = Resources.Load<Sprite>(planetName);

        if (sprite != null)
            planetImage.sprite = sprite;
        else
            Debug.LogWarning("Sprite não encontrado.");
    }

    public void LoadPlanetInfoToCanvas(string planetName)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvas.SetActive(true);
        title.text = PlanetInfoRepository.PlanetPortugueseName[planetName];
        PlanetOrbit.SetPaused(true);

        if (PlanetInfoRepository.planetDescriptions == null)
        {
            PlanetInfoRepository.LoadPlanetInfo();
        }

        LoadImage(planetName);

        description.text = PlanetInfoRepository.planetDescriptions[planetName];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
