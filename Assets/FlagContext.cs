using UnityEngine;

[CreateAssetMenu(fileName = "FlagContext", menuName = "Scriptable Objects/New FlagContext")]
public class FlagContext : ScriptableObject
{
	public Color[] colors;
	public Texture2D flag;
	public string countryName;
}
