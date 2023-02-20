using Photon.Bolt;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ThirdPersonGadgetColor : EntityBehaviour<IPlayerState>
{
    public override void Attached()
    {
        state.AddCallback("Color", ChangeColor);
    }

    private void ChangeColor()
    {
        var sword = GetComponentInChildren<GlowingSwords.Scripts.GlowingSword>(true);

        if (sword != null)
        {
            sword.BladeColor = state.Color;
        }
    }
}