using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BerryDataLoader : EditorWindow
{
    private TextAsset csvFile;
    private DefaultAsset outputFolder;
    private Vector2 scrollPos;
    private string lastLog = "";

    [MenuItem("Tools/Berry Data Loader")]
    public static void ShowWindow()
    {
        var window = GetWindow<BerryDataLoader>("Berry Data Loader");
        window.minSize = new Vector2(400, 250);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Berry Data Loader", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Drag in a CSV file and the folder where BerryData assets live. " +
            "Existing assets are matched by ID; missing ones are created automatically.",
            MessageType.Info);

        EditorGUILayout.Space(4);

        csvFile = (TextAsset)EditorGUILayout.ObjectField(
            "CSV File", csvFile, typeof(TextAsset), false);

        outputFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Output Folder", outputFolder, typeof(DefaultAsset), false);

        EditorGUILayout.Space(8);

        EditorGUI.BeginDisabledGroup(csvFile == null || outputFolder == null);
        if (GUILayout.Button("Import CSV → BerryData Assets", GUILayout.Height(30)))
        {
            lastLog = Import();
        }
        EditorGUI.EndDisabledGroup();

        if (!string.IsNullOrEmpty(lastLog))
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(200));
            EditorGUILayout.HelpBox(lastLog, MessageType.None);
            EditorGUILayout.EndScrollView();
        }
    }

    // ------------------------------------------------------------------
    // Core import logic
    // ------------------------------------------------------------------

    private string Import()
    {
        string folderPath = AssetDatabase.GetAssetPath(outputFolder);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            return "ERROR: Selected output path is not a valid folder.";
        }

        // Index existing BerryData assets in the target folder by their ID field.
        var existing = LoadExistingAssets(folderPath);

        string[] lines = csvFile.text
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Split('\n');

        if (lines.Length < 2)
            return "ERROR: CSV has no data rows.";

        int created = 0;
        int updated = 0;
        var warnings = new List<string>();

        // Skip the header row (index 0).
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Split on comma — but only the FIRST comma and the SECOND comma matter.
            // Format: ID , Name|Name , Desc|Desc
            string[] columns = SplitCsvRow(line);

            if (columns.Length < 2)
            {
                warnings.Add($"Row {i + 1}: Not enough columns, skipped.");
                continue;
            }

            string id = columns[0].Trim();
            string[] names = ParsePipeField(columns[1]);
            string[] descriptions = columns.Length >= 3
                ? ParsePipeField(columns[2])
                : new string[0];

            if (string.IsNullOrEmpty(id))
            {
                warnings.Add($"Row {i + 1}: Empty ID, skipped.");
                continue;
            }

            // Find or create the asset.
            bool isNew = false;
            if (!existing.TryGetValue(id, out BerryData asset))
            {
                asset = ScriptableObject.CreateInstance<BerryData>();
                asset.ID = id;
                string assetPath = $"{folderPath}/{id}_BerryData.asset";
                AssetDatabase.CreateAsset(asset, assetPath);
                existing[id] = asset;
                isNew = true;
                created++;
            }

            // Apply data.
            asset.name = $"{id}_BerryData";
            asset.ID = id;
            // Use SerializedObject so Undo works and the asset is marked dirty.
            var so = new SerializedObject(asset);

            SetStringArray(so, "localizedNames", names);
            SetStringArray(so, "localizedDescriptions", descriptions);

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);

            if (!isNew) updated++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Build summary.
        string summary = $"Done — {created} created, {updated} updated.";
        if (warnings.Count > 0)
            summary += "\n\nWarnings:\n" + string.Join("\n", warnings);

        Debug.Log("[BerryDataLoader] " + summary);
        return summary;
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private static Dictionary<string, BerryData> LoadExistingAssets(string folderPath)
    {
        var dict = new Dictionary<string, BerryData>();
        string[] guids = AssetDatabase.FindAssets("t:BerryData", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<BerryData>(path);
            if (asset != null && !string.IsNullOrEmpty(asset.ID))
            {
                dict[asset.ID] = asset;
            }
        }

        return dict;
    }

    private static string[] SplitCsvRow(string row)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        for (int i = 0; i < row.Length; i++)
        {
            char c = row[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // Peek ahead: escaped quote ("") or end of field?
                    if (i + 1 < row.Length && row[i + 1] == '"')
                    {
                        current.Append('"');
                        i++; // skip the second quote
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
        }

        fields.Add(current.ToString());
        return fields.ToArray();
    }

    private static string[] ParsePipeField(string field)
    {
        return field.Split('|')
            .Select(s => s.Trim())
            .ToArray();
    }

    private static void SetStringArray(SerializedObject so, string propertyName, string[] values)
    {
        SerializedProperty prop = so.FindProperty(propertyName);
        if (prop == null || !prop.isArray)
        {
            Debug.LogWarning($"[BerryDataLoader] Property '{propertyName}' not found or is not an array.");
            return;
        }

        prop.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
        {
            prop.GetArrayElementAtIndex(i).stringValue = values[i];
        }
    }
}