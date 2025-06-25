using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlanetInfoRepository
{
    static PlanetInfoRepository()
    {
        LoadPlanetInfo();
    }

    public static readonly string[] solarSystemOrder = new string[]
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

    public static readonly Dictionary<string, float> planetRadiiKm = new Dictionary<string, float>
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

    public static readonly Dictionary<string, string> PlanetPortugueseName = new Dictionary<string, string>
    {
        {"sun", "Sol"},
        {"mercury", "Mercúrio"},
        {"venus", "Vênus"},
        {"earth", "Terra"},
        {"mars", "Marte"},
        {"jupiter", "Júpiter"},
        {"saturn", "Saturno"},
        {"uranus", "Urano"},
        {"neptune", "Netuno"}
    };

    public static Dictionary<string, string> planetDescriptions = new Dictionary<string, string>();

    public static void LoadPlanetInfo()
    {
        planetDescriptions = new Dictionary<string, string>();

        string path = Path.Combine(Application.streamingAssetsPath, "planetInfo.json");

#if UNITY_ANDROID && !UNITY_EDITOR
    // No Android, usa UnityWebRequest
    UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
    www.SendWebRequest();

    while (!www.isDone) { } // Espera terminar (bloqueante, pode ser otimizado com coroutine)

    if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
    {
        Debug.LogWarning("Erro ao carregar planetInfo.json: " + www.error);
        return;
    }

    string json = www.downloadHandler.text;
#else
        // Editor ou outras plataformas
        if (!File.Exists(path))
        {
            Debug.LogWarning("planetInfo.json não encontrado em: " + path);
            return;
        }

        string json = File.ReadAllText(path);
#endif

        PlanetInfoData[] infos = JsonUtility.FromJson<PlanetInfoDataArray>("{\"items\":" + json + "}").items;

        foreach (var info in infos)
        {
            planetDescriptions[info.name.ToLower()] = info.description;
        }
    }

}

[System.Serializable]
public class PlanetInfoData
{
    public string name;
    public string description;
}

[System.Serializable]
public class PlanetInfoDataArray
{
    public PlanetInfoData[] items;
}