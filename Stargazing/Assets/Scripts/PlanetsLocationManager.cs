using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public static class PlanetsLocationManager
{
    static Dictionary<string, PlanetPositionValue> planetsPositions;

    static PlanetsLocationManager()
    {
        planetsPositions = new Dictionary<string, PlanetPositionValue>();
        Initialize();
    }

    public static PlanetPositionValue GetPlanetPositionById(string planetId)
    {
        planetsPositions.TryGetValue(planetId, out PlanetPositionValue positionValue);
        return positionValue;
    }

    private static void Initialize()
    {
        string json = File.ReadAllText("planetsLocations.json");

        Root root = JsonConvert.DeserializeObject<Root>(json);
        var result = new Dictionary<string, PlanetPositionValue>();

        foreach (var row in root.data.table.rows)
        {
            string planetId = row.entry.id;

            if (!planetsPositions.ContainsKey(planetId))
            {
                planetsPositions[planetId] = new PlanetPositionValue();
            } else
            {
                continue;
            }

            foreach (var cell in row.cells)
            {
                if (cell.position?.horizontal != null)
                {
                    var altitude = double.Parse(cell.position.horizontal.altitude.degrees);
                    var azimuth = double.Parse(cell.position.horizontal.azimuth.degrees);
                    var distance = cell.distance.fromEarth.km;

                    result[planetId] = new PlanetPositionValue
                    {
                        Altitude = altitude,
                        Azimuth = azimuth,
                        Distance = distance
                    };
                }
            }
        }
    }
}

public class PlanetPositionValue
{
    /// <summary>
    /// Altitude do planeta em relação a posição atual
    /// </summary>
    public double Altitude { get; set; }

    /// <summary>
    /// Azimuth do planeta em relação a posição atual
    /// </summary>
    public double Azimuth { get; set; }

    /// <summary>
    /// Distância em quilometros do planeta
    /// </summary>
    public double Distance { get; set; }

}

public class Root
{
    public Data data { get; set; }
}

public class Data
{
    public Table table { get; set; }
}

public class Table
{
    public List<Row> rows { get; set; }
}

public class Row
{
    public Entry entry { get; set; }
    public List<Cell> cells { get; set; }
}

public class Entry
{
    public string id { get; set; }
}

public class Cell
{
    public string date { get; set; }
    public string id { get; set; }
    public PositionData position { get; set; }
    public Distance distance { get; set; }
}

public class Distance
{
    public FromEarth fromEarth { get; set; }
}

public class FromEarth
{
    public double km { get; set; }
}

public class PositionData
{
    public Horizontal horizontal { get; set; }
}

public class Horizontal
{
    public DegreeValue altitude { get; set; }
    public DegreeValue azimuth { get; set; }
}

public class DegreeValue
{
    public string degrees { get; set; }
}
