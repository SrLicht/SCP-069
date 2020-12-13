using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirror;
using Exiled.API.Features;
using System.Threading.Tasks;

namespace Scp069.EventHandlers
{
    public static class Extension
    {
        /// <summary>
        /// Thanks Sanyae for this amazing method, love you so much
        /// </summary>
        /// <param name="player"></param>
        /// <param name="behaviorOwner"></param>
        /// <param name="targetType"></param>
        /// <param name="customSyncVar"></param>
        public static void SendCustomSyncVar(this ReferenceHub player, NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncVar)
        {
            NetworkWriter writer = NetworkWriterPool.GetWriter();
            NetworkWriter writer2 = NetworkWriterPool.GetWriter();
            MakeCustomSyncVarWriter(behaviorOwner, targetType, customSyncVar, writer, writer2);
            NetworkServer.SendToClientOfPlayer(player.networkIdentity, new UpdateVarsMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Recycle(writer);
            NetworkWriterPool.Recycle(writer2);
        }

        /// <summary>
        /// Thanks Sanyae for this amazing method, love you so much
        /// </summary>
        /// <param name="player"></param>
        /// <param name="behaviorOwner"></param>
        /// <param name="targetType"></param>
        /// <param name="customSyncVar"></param>
        public static void MakeCustomSyncVarWriter(NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncVar, NetworkWriter owner, NetworkWriter observer)
        {
            ulong dirty = 0ul;
            ulong dirty_o = 0ul;
            NetworkBehaviour behaviour = null;
            for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
            {
                behaviour = behaviorOwner.NetworkBehaviours[i];
                if (behaviour.GetType() == targetType)
                {
                    dirty |= 1UL << i;
                    if (behaviour.syncMode == SyncMode.Observers)
                        dirty_o |= 1UL << i;
                }
            }
            owner.WritePackedUInt64(dirty);
            observer.WritePackedUInt64(dirty & dirty_o);

            int position = owner.Position;
            owner.WriteInt32(0);
            int position2 = owner.Position;

            behaviour.SerializeObjectsDelta(owner);
            customSyncVar(owner);
            int position3 = owner.Position;
            owner.Position = position;
            owner.WriteInt32(position3 - position2);
            owner.Position = position3;

            if (dirty_o != 0ul)
            {
                ArraySegment<byte> arraySegment = owner.ToArraySegment();
                observer.WriteBytes(arraySegment.Array, position, owner.Position - position);
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;


            }
        }
    }
}
