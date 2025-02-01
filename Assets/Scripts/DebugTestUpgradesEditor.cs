using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugTestUpgrades))]
public class DebugTestUpgradesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default fields (this shows 'upgradeSOs' array in Inspector, etc.)
        DrawDefaultInspector();

        // Add a space or divider for clarity
        EditorGUILayout.Space();

        // Create a button in the Inspector
        if (GUILayout.Button("Load All UpgradeSOs"))
        {
            LoadAllUpgradeSOs();
        }
    }

    private void LoadAllUpgradeSOs()
    {
        // Get a reference to the target script
        DebugTestUpgrades upgradeManager = (DebugTestUpgrades)target;

        // Use the AssetDatabase to find all assets of type 'UpgradeSO'
        string[] guids = AssetDatabase.FindAssets("t:UpgradeSO");

        // Allocate an array of the correct size
        upgradeManager.upgradeLibrary = new UpgradeSO[guids.Length];

        // Loop through each GUID, load the asset, and store it in the array
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            upgradeManager.upgradeLibrary[i] = AssetDatabase.LoadAssetAtPath<UpgradeSO>(path);
        }

        // Mark the object as dirty so the changes are saved
        EditorUtility.SetDirty(upgradeManager);

        Debug.Log($"Loaded {upgradeManager.upgradeLibrary.Length} UpgradeSO assets.");
    }
}
