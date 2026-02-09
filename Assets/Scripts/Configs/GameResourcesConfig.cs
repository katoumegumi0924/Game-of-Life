using UnityEngine;

[CreateAssetMenu(fileName = "GameResourcesConfig", menuName = "GameOfLife/GameConfig/GameResourcesConfig")]
public class GameResourcesConfig : ScriptableObject
{
    [Header("Rendering Assets")]
    public GameObject displayImagePrefab;
    public Material paint;
}
