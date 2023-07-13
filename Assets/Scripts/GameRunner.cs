using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRunner : MonoBehaviour
{
    private List<NPC> NPCs = new List<NPC>();
    private List<Location> locations = new List<Location>();

    public bool isOnAssignment = false;
    public Location targetDestination;

    private static bool startGhostRace = false;
    private GhostSystem ghostSystem;

    private void Start()
    {
        ghostSystem = GameObject.Find("GhostSystem").GetComponent<GhostSystem>();

        if (startGhostRace)
        {
            var player = GameObject.Find("Player").GetComponent<PlayerControl>();
            ghostSystem.ApplyStartStateToTarget(player);

            ghostSystem.StartPlayback();

            startGhostRace = false;
        }
    }

    public void RegisterNPC(NPC npc)
    {
        NPCs.Add(npc);
        npc.SetParticlesActive(true);
    }

    public void RegisterLocation(Location location)
    {
        locations.Add(location);
    }

    public void SetLocationForNPC(NPC npc)
    {
        targetDestination = locations[Random.Range(0, locations.Count)];
        targetDestination.SetActiveLocation(true);
    }

    public void SetAllNPCParticles(bool particlesOn)
    {
        foreach (var npc in NPCs)
        {
            if(npc != null && npc.gameObject != null)
            {
                npc.SetParticlesActive(particlesOn);
            }
        }
    }

    public void StartGhostRace()
    {
        if (ghostSystem.IsRecording)
        {
            ghostSystem.ToggleRecording();
        }

        startGhostRace = true;
        SceneManager.LoadScene(0);
    }
}