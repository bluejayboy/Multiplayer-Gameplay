using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public class PlayerHandColor : EntityBehaviour<IPlayerState>
    {
        private Player player;
        private SkinnedMeshRenderer rend;

        private void Awake()
        {
            player = GetComponentInParent<Player>();
            rend = GetComponent<SkinnedMeshRenderer>();
        }

        public override void Attached()
        {
#if ISDEDICATED
            return;
#endif

            var token = entity.AttachToken as PlayerToken;

            if (token == null)
            {
                return;
            }

            player = player ?? GetComponentInParent<Player>();
            rend = rend ?? GetComponent<SkinnedMeshRenderer>();

            var data = player.List.Datas[token.Creature];

            if (rend.materials.Length > 0)
            {
                SetColor(0, data.IsFixedSkinColor ? data.RightHand : token.RightHandColor, data.IsFixedSkinColor);
            }

            if (rend.materials.Length > 1)
            {
                SetColor(1, data.IsFixedSkinColor ? data.LeftHand : token.LeftHandColor, data.IsFixedSkinColor);
            }
        }

        private void SetColor(int index, Color32 color, bool isFixed)
        {
            if (!isFixed && !ColorCheck.Instance.DefaultColors.Contains(color))
            {
                color = ColorCheck.Instance.DefaultColors[0];
            }

            rend.materials[index].SetColor("_Color", color);
        }
    }
}