using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (displays all existing fields like the original Inspector)
        DrawDefaultInspector();

        // Get reference to the target GameManager instance
        GameManager gameManager = (GameManager)target;

        // Add a section for custom buttons for each CarPrefabEntry
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Car Prefab Controls", EditorStyles.boldLabel);

        for (int i = 0; i < gameManager.carPrefabs.Count; i++)
        {
            var carPrefab = gameManager.carPrefabs[i];

            // Add a button for cloning the CarPrefabEntry
            if (GUILayout.Button($"Clone {carPrefab.carName}"))
            {
                CloneCarPrefabEntry(gameManager, carPrefab);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Save and Load Controls", EditorStyles.boldLabel);

        // Create a horizontal layout for buttons
        EditorGUILayout.BeginHorizontal();

        // Add "Load" button to the left
        if (GUILayout.Button("Load", GUILayout.MaxWidth(80)))
        {
            gameManager.filePath = Path.Combine(Application.persistentDataPath, "carData.json");

            gameManager.LoadCarData();
        }

        // Add "Save" button to the right
        if (GUILayout.Button("Save", GUILayout.MaxWidth(80)))
        {
            gameManager.SaveCarData();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void CloneCarPrefabEntry(GameManager gameManager, GameManager.CarPrefabEntry original)
    {
        // Clone the CarPrefabEntry
        GameManager.CarPrefabEntry clonedCarPrefab = new GameManager.CarPrefabEntry()
        {
            carName = original.carName + " (Clone)", // Add "(Clone)" to the name
            Object = original.Object,
            maxtorque = original.maxtorque,
            torque = original.torque,
            braketorque = original.braketorque,
            handbraketorque = original.handbraketorque,
            steeringmax = original.steeringmax,
            maxspeed = original.maxspeed,
            steeringcurve = original.steeringcurve,
            tractioncutoff = original.tractioncutoff,
            finaldriveaxle = original.finaldriveaxle,
            drivingwheels = original.drivingwheels,
            mass = original.mass,
            wheelradius = original.wheelradius,
            wheelmass = original.wheelmass
        };

        // Add the cloned entry to the list
        gameManager.carPrefabs.Add(clonedCarPrefab);

        // Mark the GameManager as dirty to save changes in the Editor
        EditorUtility.SetDirty(gameManager);
    }
}


