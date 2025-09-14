using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FlagContext", menuName = "Scriptable Objects/New FlagContext")]
public class FlagContext : ScriptableObject
{
    public Color[] colors = new Color[] { Color.white };
    public Texture2D visualFlag;
    public string countryName;
    public CountryDifficulty difficulty;
    public Continent countryContinent;

#if UNITY_EDITOR
    private void Awake()
    {
        if (visualFlag == null)
        {
            var contextDB = Resources.LoadAll<Texture2D>("FlagVisuals");

            var flag = contextDB.Where(f => f.name == name).First();
            if (flag != null)
            {
                visualFlag = flag;
                EditorUtility.SetDirty(this);
            }
        }

        if (countryName == null || countryName == "")
        {
            var lines = File.ReadAllLines(Application.dataPath + "/Resources/CountryISOs.csv");

            var countries = lines
            .Skip(1)
            .Select(line => line.Split(','))
            .Select(parts => new { Name = parts[0].Trim(), Code = parts[1].Trim() });

            // Lookup example


            countryName = countries
                .Where(c => c.Code == name)
                .Select(c => c.Name)
                .FirstOrDefault();
        }
        EditorUtility.SetDirty(this);
    }

#endif
}

