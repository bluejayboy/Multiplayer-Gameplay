using UnityEngine;
using Photon.Bolt;

public class Nuke : EntityBehaviour<IProjectileState>
{
    public Rigidbody newRigidbody;
    public GameObject bomb;
    public GameObject explosion;

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        state.AddCallback("HasExploded", ExplodeEffect);

        if (entity.IsOwner)
        {
            state.HasExploded = false;
            newRigidbody.isKinematic = false;
        }
    }

    private void ExplodeEffect()
    {
        if (state.HasExploded)
        {
            bomb.SetActive(false);
            explosion.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!state.HasExploded)
        {
            state.HasExploded = true;
            newRigidbody.isKinematic = true;
            transform.localPosition = Vector3.zero;
        }
    }
}