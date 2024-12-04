using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Create the inventory
        ICar sportcar = new Car().SetName("SRT Challenger").SetBrakeTorque(5000);
        Inventory sportcarinv = new Inventory(sportcar);

        CreatePlayerCar(sportcarinv);

        CreateBotCar(sportcarinv);
        
    }
    private void CreatePlayerCar(Inventory inventory)
    {
        var playerCar = Instantiate(carPrefab);
        playerCar.GetComponent<CarController>().InitializeCar(inventory.createcar("PlayerCar"));
    }

    private void CreateBotCar(Inventory inventory)
    {
        var botCar = Instantiate(carPrefab);
        botCar.GetComponent<CarController>().InitializeCar(inventory.createcar("BotCar"));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
