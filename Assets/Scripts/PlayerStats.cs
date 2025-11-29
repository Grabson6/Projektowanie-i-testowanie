using UnityEngine;

[CreateAssetMenu(fileName = "NoweStatystyki", menuName = "Game/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Ustawienia Ruchu")]
    public float runSpeed = 10f;
    public float jumpPower = 15f;

    [Header("Wall Mechanics")]
    public float wallJumpX = 14f;
    public float wallJumpY = 16f;
    public float wallJumpTime = 0.3f;

    [Header("Physics")]
    public float checkRadius = 0.2f;
}