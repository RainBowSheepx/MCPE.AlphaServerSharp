using SpoongePE.Core.Game.entity.impl;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.entity
{
    public class EntityTracker
    {
        private HashSet<EntityTrackerEntry> trackedEntitySet = new HashSet<EntityTrackerEntry>();
        private MCHash trackedEntityHashTable = new MCHash();
        private World world;
        private int maxTrackingDistanceThreshold;
        private int dimension;

        public EntityTracker(World var1, int var2)
        {
            this.world = var1;
            this.dimension = var2;
            this.maxTrackingDistanceThreshold = 10 * 16 - 16;
        }
        public void trackEntity(Entity var1)
        {
            if (var1 is Player) {
                this.trackEntity(var1, 512, 2);
                Player var2 = (Player)var1;
                foreach (EntityTrackerEntry var4 in trackedEntitySet)
                {
                    if (var4.trackedEntity != var2)
                    {
                        var4.updatePlayerEntity(var2);
                    }
                }
            } else if (var1 is EntityArrow) {
                this.trackEntity(var1, 64, 20, false);
            } else if (var1 is EntitySnowball) {
                this.trackEntity(var1, 64, 10, true);
            } else if (var1 is EntityEgg) {
                this.trackEntity(var1, 64, 10, true);
            } else if (var1 is EntityItem) {
                this.trackEntity(var1, 64, 20, true);
            } else if (var1 is EntityMinecart) {
                this.trackEntity(var1, 160, 5, true);
            } else if (var1 is IAnimals) {
                this.trackEntity(var1, 160, 3);
            } else if (var1 is EntityTNTPrimed) {
                this.trackEntity(var1, 160, 10, true);
            } else if (var1 is EntityFallingSand) {
                this.trackEntity(var1, 160, 20, true);
            } else if (var1 is EntityPainting) {
                this.trackEntity(var1, 160, int.MaxValue, false);
            }

        }
        public void trackEntity(Entity var1, int var2, int var3)
        {
            this.trackEntity(var1, var2, var3, false);
        }

        public void trackEntity(Entity var1, int var2, int var3, bool var4)
        {
            if (var2 > this.maxTrackingDistanceThreshold)
            {
                var2 = this.maxTrackingDistanceThreshold;
            }

            if (this.trackedEntityHashTable.ContainsItem(var1.EntityID))
            {
                throw new Exception("Entity is already tracked!");
            }
            else
            {
                EntityTrackerEntry var5 = new EntityTrackerEntry(var1, var2, var3, var4);
                this.trackedEntitySet.Add(var5);
                this.trackedEntityHashTable.AddKey(var1.EntityID, var5);
                var5.updatePlayerEntities(world.players.Values.ToList());
            }
        }

        public void untrackEntity(Entity var1)
        {
            if (var1 is Player) {
                Player var2 = (Player)var1;
                foreach (var var4 in trackedEntitySet)
                {
                    var4.removeFromTrackedPlayers(var2);
                }
            }

            EntityTrackerEntry var5 = (EntityTrackerEntry)this.trackedEntityHashTable.RemoveObject(var1.EntityID);
            if (var5 != null)
            {
                this.trackedEntitySet.Remove(var5);
                var5.sendDestroyEntityPacketToTrackedPlayers();
            }

        }

        public void updateTrackedEntities()
        {
            List<object> var1 = new List<object>();
            foreach (EntityTrackerEntry var3 in this.trackedEntitySet)
            {
                var3.updatePlayerList(world.players.Values.ToList());
                if (var3.playerEntitiesUpdated && var3.trackedEntity is Player)
                {
                    var1.Add((Player)var3.trackedEntity);
                }
            }
            for (int var6 = 0; var6 < var1.Count; var6++)
            {
                Player var7 = (Player)var1[var6];
                foreach (EntityTrackerEntry var5 in this.trackedEntitySet)
                {
                    if (var5.trackedEntity != var7)
                    {
                        var5.updatePlayerEntity(var7);
                    }
                }
            }
        }

        public void sendPacketToTrackedPlayers(Entity var1, MinecraftPacket var2)
        {
            EntityTrackerEntry var3 = (EntityTrackerEntry)this.trackedEntityHashTable.Lookup(var1.EntityID);
            if (var3 != null)
            {
                var3.sendPacketToTrackedPlayers(var2);
            }
        }

        public void sendPacketToTrackedPlayersAndTrackedEntity(Entity var1, MinecraftPacket var2)
        {
            EntityTrackerEntry var3 = (EntityTrackerEntry)this.trackedEntityHashTable.Lookup(var1.EntityID);
            if (var3 != null)
            {
                var3.sendPacketToTrackedPlayersAndTrackedEntity(var2);
            }

        }

        public void removeTrackedPlayerSymmetric(Player var1)
        {
            foreach (EntityTrackerEntry var3 in this.trackedEntitySet)
            {
                var3.removeTrackedPlayerSymmetric(var1);
            }

        }
    }
}
