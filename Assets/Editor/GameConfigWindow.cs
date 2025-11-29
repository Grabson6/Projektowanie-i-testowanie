using UnityEngine;
using UnityEditor;

public class GameConfigWindow : EditorWindow
{
    private PlayerStats targetStats;

    [MenuItem("Gra/Ustawienia Gracza")]
    public static void OpenWindow()
    {
        GetWindow<GameConfigWindow>("Konfiguracja");
    }

    void OnGUI()
    {
        GUILayout.Label("Panel Sterowania Grą", EditorStyles.boldLabel);
        GUILayout.Space(5);

        targetStats = (PlayerStats)EditorGUILayout.ObjectField("Plik danych", targetStats, typeof(PlayerStats), false);

        if (targetStats != null)
        {
            GUILayout.Space(10);

            targetStats.runSpeed = EditorGUILayout.Slider("Predkosc", targetStats.runSpeed, 1f, 30f);
            targetStats.jumpPower = EditorGUILayout.Slider("Skok", targetStats.jumpPower, 5f, 30f);

            GUILayout.Space(5);

            targetStats.wallJumpX = EditorGUILayout.FloatField("Odbicie X", targetStats.wallJumpX);
            targetStats.wallJumpY = EditorGUILayout.FloatField("Odbicie Y", targetStats.wallJumpY);

            GUILayout.Space(15);

            if (GUILayout.Button("Resetuj"))
            {
                targetStats.runSpeed = 10f;
                targetStats.jumpPower = 15f;
                targetStats.wallJumpX = 14f;
                targetStats.wallJumpY = 16f;
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(targetStats);
            }
        }
    }
}