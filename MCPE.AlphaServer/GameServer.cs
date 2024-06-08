using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using SpoongePE.Core.Game;
using SpoongePE.Core.Game.entity.impl;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Network;
using SpoongePE.Core.RakNet;
using SpoongePE.Core.Utils;

namespace SpoongePE.Core;

public class GameServer : IConnectionHandler
{
    const int PROTOCOL = 14; // Beta 1.7.3 moment

    private readonly List<string> BadUsernames = new() {
        "server",
        "rcon",
        "console",
        "herobrine",
        "astolfo",
    };

    public readonly ServerWorld ServerWorld;

    public GameServer(World world)
    {
        ServerWorld = new ServerWorld(this, world);
    }

    public void OnOpen(RakNetClient client)
    {
        Logger.Debug($"[+] {client}");
    }

    public void OnClose(RakNetClient client, string reason)
    {
        Logger.Debug($"[-] {client} ({reason})");

        ServerWorld.RemovePlayer(client, reason);
    }

    public void OnData(RakNetClient client, ReadOnlyMemory<byte> data)
    {
        var packet = MinecraftPacket.Parse(data);
        var packetName = packet.GetType().Name[0..^6];
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var handlerMethod = GetType().GetMethod($"Handle{packetName}", bindingFlags);
        var genericHandlerMethod = GetType().GetMethod("HandleGeneric", bindingFlags);
        //  Logger.Debug($"Handler implemented for {packetName}. {handlerMethod} {genericHandlerMethod}");
        if (handlerMethod == null)
            Logger.Warn($"Handler not implemented for {packetName}. Bug?");

        handlerMethod?.Invoke(this, new object[] { client, packet });
        genericHandlerMethod?.Invoke(this, new object[] { client, packet });
    }

    public void OnUpdate() { }

    public virtual void HandleLoginRequest(RakNetClient client, LoginRequestPacket packet)
    {
        var responseStatus = LoginResponsePacket.StatusFor(packet.Protocol1, packet.Protocol2, PROTOCOL);
        var shouldRejectLogin = BadUsernames.Contains(packet.Username.ToLower())
                                || ServerWorld.GetByName(packet.Username) != null // Already logged in.
                                || ServerWorld.Players.Count() >= RakNetServer.Properties.maxPlayers
                                || packet.Username.Length > 16;
        foreach (char c in packet.Username)
        {
            if (!Char.IsAscii(c))
            {
                shouldRejectLogin = true;
                break;
            }
        }
        if (shouldRejectLogin) responseStatus = LoginResponsePacket.LoginStatus.ClientOutdated;

        client.Send(new LoginResponsePacket { Status = responseStatus });

        if (responseStatus != LoginResponsePacket.LoginStatus.VersionsMatch)
            return;

        // We can log in, start the game. 
        var newPlayer = ServerWorld.AddPlayer(client, packet.ClientId, packet.Username);

        Logger.Info($"{packet.Username} joined the game: Position: {newPlayer.Position}");

        client.Send(new StartGamePacket
        {
            Seed = ServerWorld.World.Seed,
            Pos = newPlayer.Position,
            EntityId = newPlayer.EntityID,
            Gamemode = 1,
        }
        );

        client.Send(new ChatPacket
        {
            Message = RakNetServer.Properties.motd.Replace("@player", newPlayer.Username).Replace("@servername", RakNetServer.Properties.serverName).Replace("@desc", RakNetServer.Properties.description),
        }
        );
    }

    public virtual void HandleReady(RakNetClient client, ReadyPacket packet)
    {
        // Notify this new player about other existing players.

        foreach (var player in ServerWorld.Players)
        {
            if (player.IsClientOf(client))
                continue;
            /*            client.Send(new PlayerEquipmentPacket
                        {
                            EntityId = player.EntityID,
                            itemID = 0
                        });*/

            client.Send(new AddPlayerPacket
            {
                PlayerId = player.PlayerID,
                Username = player.Username,
                EntityId = player.EntityID,
                Pos = player.Position,
                Pitch = (byte)(player.ViewAngle.Y / 360.0f * 256.0f),
                Yaw = (byte)(player.ViewAngle.X / 360.0f * 256.0f),
            }
            );
        }

        client.Send(new ContainerSetContentPacket
        {
            WindowId = 0,
            Items = new List<ItemStack> {
                    new ItemStack { itemID = 256, stackSize = 10 },
                },
            Hotbar = new List<int> { 0 },
        }
        );

        client.Send(new SetTimePacket
        {
            Time = (int)ServerWorld.World.worldTime,
        }
        );


    }

    public virtual void HandleMessage(RakNetClient client, MessagePacket packet) { 
        
        
        ServerWorld.SendAll(packet);
        if (packet.Message.Contains("h"))
        {
            Console.WriteLine("Spawn pig");
            EntityPig pig = new EntityPig(ServerWorld.World);
            pig.setLocationAndAngles(client.player.posX, client.player.posY, client.player.posZ, client.player.rotationYaw, client.player.rotationPitch);
           Console.WriteLine(this.ServerWorld.World.entityJoinedWorld(pig));
        }
    
    }

    //public virtual void HandleAddEntity(RakNetClient client, AddEntityPacket packet) { }
    //public virtual void HandleRemoveEntity(RakNetClient client, RemoveEntityPacket packet) { }
    //public virtual void HandleAddItemEntity(RakNetClient client, AddItemEntityPacket packet) { }
    //public virtual void HandleTakeItemEntity(RakNetClient client, TakeItemEntityPacket packet) { }
    //public virtual void HandleMoveEntity(RakNetClient client, MoveEntityPacket packet) { }
    //public virtual void HandleMoveEntityPosRot(RakNetClient client, MoveEntityPosRotPacket packet) { }
    //public virtual void HandleRotateHead(RakNetClient client, RotateHeadPacket packet) { }

    public virtual void HandleMovePlayer(RakNetClient client, MovePlayerPacket packet) =>
        ServerWorld.MovePlayer(client, packet.Pos, packet.Rot);

    public virtual void HandlePlaceBlock(RakNetClient client, PlaceBlockPacket packet)
    {

        ServerWorld.World.placeBlockAndNotifyNearby(packet.X, packet.Y, packet.Z, packet.Block, packet.Meta);
    }

    public const int PLACE_DISTANCE_SQUARED = 6 * 6;
    public virtual void HandleRemoveBlock(RakNetClient client, RemoveBlockPacket packet)
    {
        int diffX = ((int)client.player.posX - packet.X);
        int diffY = ((int)client.player.posY - packet.Y);
        int diffZ = ((int)client.player.posZ - packet.Z);
        if (diffX * diffX + diffY * diffY + diffZ * diffZ > 36) return;
        
        ServerWorld.World.removeBlock(packet.X, packet.Y, packet.Z);
    }


    //public virtual void HandleUpdateBlock(RakNetClient client, UpdateBlockPacket packet) { }
    //public virtual void HandleAddPainting(RakNetClient client, AddPaintingPacket packet) { }
    //public virtual void HandleExplode(RakNetClient client, ExplodePacket packet) { }
    //public virtual void HandleLevelEvent(RakNetClient client, LevelEventPacket packet) { }
    //public virtual void HandleTileEvent(RakNetClient client, TileEventPacket packet) { }
    //public virtual void HandleEntityEvent(RakNetClient client, EntityEventPacket packet) { }
    public virtual void HandleRequestChunk(RakNetClient client, RequestChunkPacket rcp) =>
        ServerWorld.SendChunkFromRequest(client, rcp);
    //public virtual void HandleChunkData(RakNetClient client, ChunkDataPacket packet) { }
    public virtual void HandlePlayerEquipment(RakNetClient client, PlayerEquipmentPacket packet)
    {
        Logger.PInfo($"Slot: {packet.Slot} Block: {packet.Item} Meta: {packet.Meta} EntID: {packet.EntityId}");
    }
    //public virtual void HandlePlayerArmorEquipment(RakNetClient client, PlayerArmorEquipmentPacket packet) { }
    //public virtual void HandleInteract(RakNetClient client, InteractPacket packet) { }
    public virtual void HandleUseItem(RakNetClient client, UseItemPacket packet)
    {

        Logger.PInfo($"X: {packet.X} Y: {packet.Y} Block: {packet.Block} Meta: {packet.Meta} Id?: {packet.Id} Pos: {packet.Pos} FacePos: {packet.FPos} ");
    }
    //public virtual void HandlePlayerAction(RakNetClient client, PlayerActionPacket packet) { }
    //public virtual void HandleHurtArmor(RakNetClient client, HurtArmorPacket packet) { }
    //public virtual void HandleSetEntityData(RakNetClient client, SetEntityDataPacket packet) { }
    //public virtual void HandleSetEntityMotion(RakNetClient client, SetEntityMotionPacket packet) { }
    //public virtual void HandleSetRiding(RakNetClient client, SetRidingPacket packet) { }

    public virtual void HandleSetHealth(RakNetClient client, SetHealthPacket packet) => ServerWorld.SendAll(new SetHealthPacket
    {
        Health = (sbyte)(packet.Health < -31 ? packet.Health + 64 : packet.Health),
    }
    );

    //public virtual void HandleSetSpawnPosition(RakNetClient client, SetSpawnPositionPacket packet) { }
    public virtual void HandleAnimate(RakNetClient client, AnimatePacket packet) => ServerWorld.SendAll(packet);
    //public virtual void HandleRespawn(RakNetClient client, RespawnPacket packet) { }
    //public virtual void HandleSendInventory(RakNetClient client, SendInventoryPacket packet) { }
    //public virtual void HandleDropItem(RakNetClient client, DropItemPacket packet) { }
    //public virtual void HandleContainerOpen(RakNetClient client, ContainerOpenPacket packet) { }
    //public virtual void HandleContainerClose(RakNetClient client, ContainerClosePacket packet) { }
    public virtual void HandleContainerSetSlot(RakNetClient client, ContainerSetSlotPacket packet)
    {

        Logger.PInfo($"windowId: {packet.WindowId} slot: {packet.Slot} item: {packet.Item}");
    }
    //public virtual void HandleContainerSetData(RakNetClient client, ContainerSetDataPacket packet) { }
    //public virtual void HandleContainerSetContent(RakNetClient client, ContainerSetContentPacket packet) { }
    //public virtual void HandleContainerAck(RakNetClient client, ContainerAckPacket packet) { }
    //public virtual void HandleChat(RakNetClient client, ChatPacket packet) { }
    //public virtual void HandleSignUpdate(RakNetClient client, SignUpdatePacket packet) { }
    //public virtual void HandleAdventureSettings(RakNetClient client, AdventureSettingsPacket packet) { }
}
