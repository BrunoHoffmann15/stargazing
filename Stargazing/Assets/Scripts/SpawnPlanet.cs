using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class SpawnPlanet : MonoBehaviour
{
    public List<GameObject> prefabObj;
    public float distanciaDaCamera = 1.5f;
    public TMP_Text title;
    public TMP_Text description;
    public GameObject canvas;
    private CanvasGroup canvasGroup;

    private GameObject sunAnchor;

    private readonly string[] solarSystemOrder = new string[]
    {
        "mercury",
        "venus",  
        "earth",  
        "mars",   
        "jupiter",
        "saturn",
        "uranus",
        "neptune"
    };

    private readonly Dictionary<string, float> planetRadiiKm = new Dictionary<string, float>
    {
        {"sun", 696340f},
        {"mercury", 2439.7f},
        {"venus", 6051.8f},
        {"earth", 6371.0f},
        {"mars", 3389.5f},
        {"jupiter", 69911f},
        {"saturn", 58232f},
        {"uranus", 25362f},
        {"neptune", 24622f}
    };

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
        canvasGroup.alpha = 0f;               // Torna invisível
        canvasGroup.interactable = false;     // Desativa interações
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
        float sunRadius = planetRadiiKm["sun"];
        float exaggerationFactor = 20f; // <--- exaggerate the differences!

        for (int i = 1; i < solarSystemOrder.Length; i++)
        {
            string planetName = solarSystemOrder[i];
            GameObject prefab = prefabObj.Find(obj => obj.name.ToLower() == planetName);

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab for {planetName} not found in prefabObj list.");
                continue;
            }

            // Exaggerated normalized scale
            float planetRadius = planetRadiiKm[planetName];
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
            
            info.canva = canvasGroup;
            info.canva2 = canvas;
            info.title = title;
            info.description = description;

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
        canvasGroup.alpha = 0f;               // Torna invisível
        canvasGroup.interactable = false;     // Desativa interações
        canvasGroup.blocksRaycasts = false;   // Bloqueia cliques
        PlanetOrbit.SetPaused(false);
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
            transform.RotateAround(sunAnchor.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
    }

    public static void SetPaused(bool pause)
    {
        paused = pause;
    }
}


[System.Serializable]
public class PlanetInfoData
{
    public string name;
    public string description;
}

public class PlanetInfo : MonoBehaviour
{
    public string planetName;
    private static Dictionary<string, string> planetDescriptions;
    private static bool showingInfo = false;
    public CanvasGroup canva;
    public GameObject canva2;
    public TMP_Text title;
    public TMP_Text description;

    void Start()
    {
        if (planetDescriptions == null)
            LoadPlanetInfo();
    }

    void OnMouseDown()
    {
        if (planetDescriptions != null && planetDescriptions.ContainsKey(planetName.ToLower()))
        {
            // Debug.Log($"Planet: {planetName}\n{planetDescriptions[planetName.ToLower()]}");
            canva.alpha = 1f;               // Torna visível
            canva.interactable = true;
            canva.blocksRaycasts = true;
            canva2.SetActive(true);
            title.text = planetName;

            PlanetOrbit.SetPaused(true);
            description.text = planetDescriptions[planetName.ToLower()];
            // Show UI panel here if needed
        }
    }

    private void LoadPlanetInfo()
    {
        planetDescriptions = new Dictionary<string, string>();
        string path = Path.Combine(Application.dataPath, "Scripts/planetInfo.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("planetInfo.json not found at: " + path);
            return;
        }
        string json = File.ReadAllText(path);
        PlanetInfoData[] infos = JsonUtility.FromJson<PlanetInfoDataArray>("{\"items\":" + json + "}").items;
        foreach (var info in infos)
        {
            planetDescriptions[info.name.ToLower()] = info.description;
        }
    }

    [System.Serializable]
    private class PlanetInfoDataArray
    {
        public PlanetInfoData[] items;
    }
}