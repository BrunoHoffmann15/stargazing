using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnPlanet : MonoBehaviour
{
    public List<GameObject> prefabObj;
    public float distanciaDaCamera = 1.5f;
    public TMP_Text title;
    public TMP_Text description;
    public GameObject canvas;
    private CanvasGroup canvasGroup;
    public Image planetImage;

    private GameObject sunAnchor;



    void Start()
    {
        SetUpCanvas();
        SpawnSun();
        SpawnPlanets();
    }

    void SetUpCanvas()
    {
        canvasGroup = canvas.GetComponent<CanvasGroup>();

        canvas.SetActive(false);
        canvasGroup.enabled = false;
        canvasGroup.alpha = 0f;               // Torna invis�vel
        canvasGroup.interactable = false;     // Desativa intera��es
        canvasGroup.blocksRaycasts = false;   // Bloqueia cliques
    }

    void SpawnSun()
    {
        var sunDistanceFromCamera = 1.5f;
        var sunScale = 0.5f;
        if (Camera.main != null && prefabObj != null)
        {
            foreach (var obj in prefabObj)
            {
                if (obj.name.ToLower() == "sun")
                {
                    Vector3 sunPosition = Camera.main.transform.position
                        + Camera.main.transform.forward * sunDistanceFromCamera;

                    sunAnchor = Instantiate(obj, sunPosition, Quaternion.identity);
                    sunAnchor.transform.localScale = Vector3.one * sunScale;
                    sunAnchor.name = "SunAnchor";
                    break;
                }
            }
        }
    }

    void SpawnPlanets()
    {
        if (sunAnchor == null) return;
        float planetDistanceStep = 1.0f;
        float currentDistance = planetDistanceStep;

        float sunScale = 0.5f; // Use the same value as in SpawnSun
        float sunRadius = PlanetInfoRepository.planetRadiiKm["sun"];
        float exaggerationFactor = 20f; // <--- exaggerate the differences!

        for (int i = 1; i < PlanetInfoRepository.solarSystemOrder.Length; i++)
        {
            string planetName = PlanetInfoRepository.solarSystemOrder[i];
            GameObject prefab = prefabObj.Find(obj => obj.name.ToLower() == planetName);

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab for {planetName} not found in prefabObj list.");
                continue;
            }

            // Exaggerated normalized scale
            float planetRadius = PlanetInfoRepository.planetRadiiKm[planetName];
            float normalizedScale = (planetRadius / sunRadius) * sunScale * exaggerationFactor;

            Vector3 planetPosition = sunAnchor.transform.position
                + sunAnchor.transform.forward * currentDistance;

            GameObject planetInstance = Instantiate(prefab, planetPosition, Quaternion.identity, sunAnchor.transform);
            planetInstance.transform.localScale = Vector3.one * normalizedScale;

            var orbit = planetInstance.AddComponent<PlanetOrbit>();
            orbit.sunAnchor = sunAnchor.transform;
            orbit.orbitSpeed = 10f + currentDistance * 5f;

            // Attach PlanetInfo and set planetName
            var info = planetInstance.AddComponent<PlanetInfo>();
            info.planetName = planetName;

            // === Criar objeto do texto ===
            GameObject labelObject = new GameObject("Label_" + planetName);
            labelObject.transform.SetParent(planetInstance.transform);

            // Posi��o *acima* do planeta (ajuste Y se quiser mais alto)
            labelObject.transform.localPosition = new Vector3(0, 1.5f, 0);
            labelObject.transform.localRotation = Quaternion.identity;

            // Adiciona o texto
            TextMeshPro textMesh = labelObject.AddComponent<TextMeshPro>();
            textMesh.text = PlanetInfoRepository.PlanetPortugueseName[planetName];
            textMesh.fontSize = 100f;
            textMesh.enableAutoSizing = true;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.color = Color.white;
            textMesh.rectTransform.sizeDelta = new Vector2(40f, 3f); // largura/altura do espa�o do texto

            // Escala fixa pequena (ajust�vel)
            if (planetRadius < 6500.0f)
            {
                labelObject.transform.localScale = Vector3.one * 0.65f;
            }
            else if (planetRadius < 30000f)
            {
                labelObject.transform.localScale = Vector3.one * 0.30f;
            }
            else
            {
                labelObject.transform.localScale = Vector3.one * 0.15f;
            }
            

            // Faz o texto olhar para a c�mera
            labelObject.AddComponent<Billboard>();

            // Garante que o planeta tenha um collider
            if (planetInstance.GetComponent<Collider>() == null)
            {
                planetInstance.AddComponent<SphereCollider>();
            }

            currentDistance += planetDistanceStep;
        }
    }

    /*
    void Start()
    {
        if (Camera.main != null && prefabObj != null)
        {

            foreach (var i in prefabObj)
            {   
                Vector3 posicaoAlvo = Camera.main.transform.position 
                    + Camera.main.transform.forward * (Random.Range(0, 10));

                Instantiate(i, posicaoAlvo, Quaternion.identity);
            }
        }
    }
    */

    // Update is called once per frame
    void Update()
    {

    }

    public void BUTTOOOONN()
    {
        canvas.SetActive(false);
        canvasGroup.enabled = false;
        canvasGroup.alpha = 0f;               // Torna invis�vel
        canvasGroup.interactable = false;     // Desativa intera��es
        canvasGroup.blocksRaycasts = false;   // Bloqueia cliques
        PlanetOrbit.SetPaused(false);
    }
}

public class Billboard : MonoBehaviour 
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); // Corrige a rota��o se o texto estiver invertido
        }
    }
}


public class PlanetOrbit : MonoBehaviour
{
    public Transform sunAnchor;
    public float orbitSpeed = 10f;
    private static List<PlanetOrbit> allOrbits = new List<PlanetOrbit>();
    public static bool paused = false;

    void OnEnable() => allOrbits.Add(this);
    void OnDisable() => allOrbits.Remove(this);

    void Update()
    {
        if (sunAnchor != null && !paused)
        {
            float newOrbitSpeedWithFactor = orbitSpeed * VelocityManager.GetVelocity();
            transform.RotateAround(sunAnchor.position, Vector3.up, newOrbitSpeedWithFactor * Time.deltaTime);
        }
    }

    public static void SetPaused(bool pause)
    {
        paused = pause;
    }
}

public class PlanetInfo : MonoBehaviour, IPointerClickHandler
{
    public string planetName;
    private static bool showingInfo = false;

    void Start()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        /*if (PlanetInfoRepository.planetDescriptions != null && PlanetInfoRepository.planetDescriptions.ContainsKey(planetName.ToLower()))
        {
            canva.alpha = 1f;
            canva.interactable = true;
            canva.blocksRaycasts = true;
            canva2.SetActive(true);
            title.text = PlanetInfoRepository.PlanetPortugueseName[planetName];
            PlanetOrbit.SetPaused(true);
            description.text = PlanetInfoRepository.planetDescriptions[planetName.ToLower()];
            LoadImage();
        }*/
    }

    /*
    void OnMouseDown()
    {
        if (planetDescriptions != null && planetDescriptions.ContainsKey(planetName.ToLower()))
        {
            // Debug.Log($"Planet: {planetName}\n{planetDescriptions[planetName.ToLower()]}");
            canva.alpha = 1f;               // Torna vis�vel
            canva.interactable = true;
            canva.blocksRaycasts = true;
            canva2.SetActive(true);
            title.text = SpawnPlanet.PlanetPortugueseName[planetName];

            PlanetOrbit.SetPaused(true);
            description.text = planetDescriptions[planetName.ToLower()];
            // Show UI panel here if needed
        }
    }*/

}