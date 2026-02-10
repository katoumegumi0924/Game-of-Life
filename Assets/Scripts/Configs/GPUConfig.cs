using UnityEngine;

[CreateAssetMenu(fileName = "GPUConfig", menuName = "GameOfLife/GameConfig/GPUConfig")]
public class GPUConfig : ScriptableObject
{
    public ComputeShader lifeShader;
}
