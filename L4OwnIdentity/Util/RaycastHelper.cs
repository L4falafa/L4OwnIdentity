using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace L4OwnIdentity.Util
{

        public class RaycastHelper
        {
            public static Player GetPlayerFromHits(Player caller, float maxDistance)
            {
                var hits = Physics.RaycastAll(new Ray(caller.look.aim.position, caller.look.aim.forward), maxDistance, RayMasks.PLAYER_INTERACT | RayMasks.PLAYER);
                Player player = null;
                for (int i = 0; i < hits.Length; i++)
                {
                    Player suspect = hits[i].transform.GetComponentInParent<Player>();
                    if (suspect != caller)
                    {
                        player = suspect;
                        break;
                    }
                }
                return player;
            }
        }

}
