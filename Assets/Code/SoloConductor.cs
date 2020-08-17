using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoloConductor : MonoBehaviour
{
    Project PreviousSolo;

    private void Awake()
    {
        Project.OnStartSolo += HandleSoloStarted;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        Project.OnStartSolo -= HandleSoloStarted;
    }

    void HandleSoloStarted(Project target)
    {
        PreviousSolo = target;
    }

    private void Start()
    {
        StartCoroutine(SoloInitiationLoop());
    }

    IEnumerator SoloInitiationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(15f, 30f));
            if (GameState.Instance.CurrentState == GameState.State.World
                && (PreviousSolo == null || !PreviousSolo.IsPlayingSolo))
            {
                var newSolo = Utils.GetRandom(GraveyardLoader.Instance.GraveParent.GetComponentsInChildren<Project>().ToList());
                //newSolo.TogglePlaying();
                yield return new WaitForSeconds(Random.Range(30f, 40f));
                if (newSolo == PreviousSolo && newSolo.IsPlayingSolo)
                {
                    //newSolo.TogglePlaying();
                }
            }
        }
    }
}
