using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public LayerMask TargetLayers;
    public float CastDistance = 2f;

    void Update()
    {
        if (GameState.Instance.CurrentState != GameState.State.World)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, CastDistance, TargetLayers))
            {
                var interaction = hit.collider.GetComponent<Interaction>();
                if (interaction)
                {
                    interaction.Activate();
                }
            }
            Debug.DrawLine(transform.position, transform.position + transform.forward * CastDistance, Color.green);
        }
    }
}
