using UI;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Stamina")] public bool CanSprint;
    [ReadOnlyAttribute] public bool IsSprinting; 
    public float Stamina, MaxStamina, StaminaDepleteRate;
    
    [Header("Camera")]public float CamSensitivity;

    [Header("UI")] public UIManager UIManager; 
}
