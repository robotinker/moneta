using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoloConductor : MonoBehaviour
{
    BuriedProject PreviousSolo;

    private void Awake()
    {
        BuriedProject.OnStartSolo += HandleSoloStarted;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        BuriedProject.OnStartSolo -= HandleSoloStarted;
    }

    void HandleSoloStarted(BuriedProject target)
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
            yield return new WaitForSeconds(UnityEngine.Random.Range(15f, 30f));
            if (GameState.Instance.CurrentState == GameState.State.World
                && (PreviousSolo == null || !PreviousSolo.IsPlaying))
            {
                var newSolo = Utils.GetRandom(GraveyardLoader.Instance.GraveParent.GetComponentsInChildren<BuriedProject>().ToList());
                newSolo.TogglePlaying();
                yield return new WaitForSeconds(UnityEngine.Random.Range(30f, 40f));
                if (newSolo == PreviousSolo && newSolo.IsPlaying)
                {
                    newSolo.TogglePlaying();
                }
            }
        }
    }
}
