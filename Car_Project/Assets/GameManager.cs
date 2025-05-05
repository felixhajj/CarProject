using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.IO;
using UnityEditor;
//using OpenCover.Framework.Model;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public string filePath;//save car data into Json

    public enum CarRole
    {
        PlayerCar,
        BotCar
    }

    //!!everything done or related to this class should be done before starting the game.
    [System.Serializable]
    public class CarPrefabEntry
    {
        public GameObject Object;
        public CarRole carRole;
        public string carName;//not of the inventory, but of this one specific car(can be the same as the inventory name)
        public int maxtorque;
        public AnimationCurve torque;
        public float braketorque;
        public float handbraketorque;
        public float steeringmax;
        public float maxspeed;
        public AnimationCurve steeringcurve;
        public float tractioncutoff;
        public float finaldriveaxle;
        public float drivingwheels;
        public bool istractioncontrol;
        public float mass;
        public float wheelradius;
        public float wheelmass;

    }
    public Dictionary<string, ICar> CarsDict = new Dictionary<string, ICar>();
    private Dictionary<string, Inventory> CarsInvDict = new Dictionary<string, Inventory>();
    //those dictionnaries are purely data related, and not directly tied to the object.

    public List<CarPrefabEntry> carPrefabs; // A list to define multiple prefabs via Unity Inspector




    void Start()
    {

    }

    public void LoadCarData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this); // Load data into this instance
        }
        
        CarsDict.Clear();
        CarsInvDict.Clear();

        foreach (var prefabEntry in carPrefabs)
        {
            if (1==1)//temporary
            {
                string uniqueCarName = EnsureUniqueCarNames(prefabEntry.carName);

                // Create a new car instance based on the CarPrefabEntry values
                ICar car = new Car()
                    .SetName(uniqueCarName)
                    .SetMaxTorque(prefabEntry.maxtorque)
                    .SetBrakeTorque(prefabEntry.braketorque)
                    .SetHandBrakeTorque(prefabEntry.handbraketorque)
                    .SetSteeringMax(prefabEntry.steeringmax)
                    .SetMaxSpeed(prefabEntry.maxspeed)
                    .SetSteeringCurve(prefabEntry.steeringcurve)
                    .SetTractionCutoff(prefabEntry.tractioncutoff)
                    .SetFinalDriveAxle(prefabEntry.finaldriveaxle)
                    .SetDrivingWheels(prefabEntry.drivingwheels)
                    .SetIsTractionControl(prefabEntry.istractioncontrol)
                    .SetMass(prefabEntry.mass)
                    .SetWheelRadius(prefabEntry.wheelradius)
                    .SetWheelMass(prefabEntry.wheelmass);

                CarsDict.Add(prefabEntry.carName, car);

                CarsInvDict.Add(prefabEntry.carName, new Inventory(car));
            }

            // Create the car in the scene
            CreateCar(prefabEntry.carRole.ToString(), CarsInvDict[prefabEntry.carName], prefabEntry);
        }

        //create default car if i do not have a car in the dictionnary with this key(name)
        if (!CarsDict.ContainsKey("SRT Challenger"))
        {
            carPrefabs.Add(new CarPrefabEntry
            {
                carName = "SRT Challenger",
                maxtorque = 656,
                braketorque = 5000,
                handbraketorque = 8000,
                steeringmax = 300,
                maxspeed = 300,
                tractioncutoff = 0.35f,
                finaldriveaxle = 300,
                drivingwheels = 2,
                mass = 1800,
                wheelradius = 0.361f,
                wheelmass = 36
            });// Add the default entry if the list is empty
        }
    }

    public void SaveCarData()
    {
        string json = JsonUtility.ToJson(this); // Convert the object to JSON
        File.WriteAllText(filePath, json); // Save to file
    }




    private void CreateCar(string carRole, Inventory inventory, CarPrefabEntry prefab)
    {
        //every class that needs an instance of Car will be called here: in this case: powertowheels ad carcontrolelr.

        // instantiate a physical object into the unity world, but i dont need it at the current moment, as the object itself is dragged into the prefab, so its available
        var carInstanceGO = Instantiate(prefab.Object);

        // Initialize the Car instance for the car using inventory
        var carInstance = inventory.createcar(carRole);

        var initializers = carInstanceGO.GetComponents<MonoBehaviour>().OfType<IcarInitializer>();


        foreach (var initializer in initializers)
        {
            initializer.Initialize(carInstance as Car);
        }


        /*
        var carController = carInstanceGO.GetComponent<CarController>();
        var powerToWheels = carInstanceGO.GetComponent<PowerToWheels>();

        if (carController != null)
        {
            carController.Initialize(carInstance as Car);
        }

        if (powerToWheels != null)
        {
            powerToWheels.Initialize(carInstance as Car);
        }
        
         */              
        
        
        //i should put SaveCarData(into Json) here, if im only adding cars occasionally, and not in abundance.
    }

    private string EnsureUniqueCarNames(string carName)
    {
        int count = 1;
        string uniqueName = carName;

        // Count how many times the carName already exists in CarsDict
        foreach (var key in CarsDict.Keys)
        {
            if (key.StartsWith(carName))
            {
                count++;
            }
        }

        // Append count only for the specific car name
        if (count > 1)
        {
            uniqueName = carName + " " + (count - 1); // Ensure it starts from 1
        }

        return uniqueName; // Return the unique name
    }





}


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

