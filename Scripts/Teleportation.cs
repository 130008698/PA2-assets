using UnityEngine;
using Photon.Pun;
public class PowerUpSpawner : MonoBehaviourPunCallbacks
{
    public GameObject powerUpPrefab1; // Assign this in the inspector
    public GameObject powerUpPrefab2;
    public GameObject powerUpPrefab3;
    private Transform planet; // Assign your planet transform here
    public float spawnHeightAbovePlanet = 1f; // Adjust based on your needs
    private float timer = 10.0f;
    void Start()
    {
        planet = GameObject.Find("forest").transform;
        
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0){
            SpawnRandomPowerUp();
            timer = 60.0f;
        }
    }
    public override void OnJoinedRoom() // Callback when the room is joined
    {
        SpawnRandomPowerUp(); // Now it's safe to spawn power-ups
    }
    void SpawnRandomPowerUp()
    {

        // Choose a random power-up prefab

        // Generate a random position on the planet
        for(int i = 0; i<3; ++i){
        Vector3 randomPositionOnSphere = Random.onUnitSphere * (planet.localScale.x * 30 * 0.5f + spawnHeightAbovePlanet);
        Vector3 spawnPosition = planet.position + randomPositionOnSphere;

        //Instantiate the power-up at the calculated position
        GameObject spawnedPowerUp1 = PhotonNetwork.Instantiate(powerUpPrefab1.name, spawnPosition, Quaternion.identity);
        randomPositionOnSphere = Random.onUnitSphere * (planet.localScale.x * 30 * 0.5f + spawnHeightAbovePlanet);
        spawnPosition = planet.position + randomPositionOnSphere;
        GameObject spawnedPowerUp2 = PhotonNetwork.Instantiate(powerUpPrefab2.name, spawnPosition, Quaternion.identity);
        randomPositionOnSphere = Random.onUnitSphere * (planet.localScale.x * 30 * 0.5f + spawnHeightAbovePlanet);
        spawnPosition = planet.position + randomPositionOnSphere;
        GameObject spawnedPowerUp3 = PhotonNetwork.Instantiate(powerUpPrefab3.name, spawnPosition, Quaternion.identity);

        // Align the power-up's up vector with the direction from the planet center to the power-up position
        spawnedPowerUp1.transform.up = (spawnedPowerUp1.transform.position - planet.position).normalized;
        spawnedPowerUp2.transform.up = (spawnedPowerUp2.transform.position - planet.position).normalized;
        spawnedPowerUp3.transform.up = (spawnedPowerUp3.transform.position - planet.position).normalized;
        }
        Debug.Log("PowerUps Spawned!");
    }
}