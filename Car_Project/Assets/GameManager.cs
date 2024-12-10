using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using System.IO;


public class GameManager : MonoBehaviour
{
    private string filePath;//save car data into Json

    //!!everything done or related to this class should be done before starting the game.
    [System.Serializable]
    public class CarPrefabEntry
    {
        public GameObject Object;
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
        public float mass;
        public float wheelradius;
        public float wheelmass;
    }
    public Dictionary<string, ICar> CarsDict = new Dictionary<string, ICar>();
    private Dictionary<string, Inventory> CarsInvDict = new Dictionary<string, Inventory>();

    public List<CarPrefabEntry> carPrefabs; // A list to define multiple prefabs via Unity Inspector

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "carData.json");
        LoadCarData();

        

        foreach (var prefabEntry in carPrefabs)
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
                .SetMass(prefabEntry.mass)
                .SetWheelRadius(prefabEntry.wheelradius)
                .SetWheelMass(prefabEntry.wheelmass);

            // Add the new car to the CarsDict
            CarsDict.Add(prefabEntry.carName, car);

            // Create and add the inventory for the car to CarsInvDict
            CarsInvDict.Add(prefabEntry.carName, new Inventory(car));

            // Create the car in the scene
            CreateCar(prefabEntry.carName, CarsInvDict[prefabEntry.carName], prefabEntry);
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
        //i put SaveCarData here so i dont have to call is multiple times, and i call it once after iterating through all the Cars
        SaveCarData();

    }

    private void OnApplicationQuit()
    {
        
        SaveCarData(); // Save data before the application quits if i ever added some extra Cars when the application is running.
    }




    private void CreateCar(string carType, Inventory inventory, CarPrefabEntry prefab)
    {

        // Instantiate the car GameObject
        var carInstanceGO = Instantiate(prefab.Object);

        // Initialize the Car instance for the car using inventory
        var carInstance = inventory.createcar(carType);

        // Initialize CarMediator with the specific Car instance
        var carMediator = carInstanceGO.GetComponent<CarMediator>();
        carMediator.Initialize(carInstance as Car);

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


    private void SaveCarData()
    {
        string json = JsonUtility.ToJson(this); // Convert the object to JSON
        File.WriteAllText(filePath, json); // Save to file
    }

    private void LoadCarData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this); // Load data into this instance
        }
    }

}
