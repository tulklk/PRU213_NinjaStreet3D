using UnityEngine;

public class VehicleSelected : MonoBehaviour
{
    public int currentVehicleIndex;
    public GameObject[] vehicles;
    void Start()
    {
        currentVehicleIndex = PlayerPrefs.GetInt("SelectedVehicle", 0); 
        foreach (GameObject vehicle in vehicles)
        {
            vehicle.SetActive(false); 
        }
        vehicles[currentVehicleIndex].SetActive(true); 
    }
}
