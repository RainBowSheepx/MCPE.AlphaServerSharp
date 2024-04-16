using SpoongePE.Core.NBT;
using SpoongePE.Core.RakNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SpoongePE.Core.Game;

public class Player : Entity
{
    public RakNetClient Client;

    public ulong PlayerID;
    public string Username;
    public string DisplayName;
    public bool inCreative;
    public Vector3 Position = new(100.0f, 10.0f, 100.0f);
    public Vector3 ViewAngle = new();
    public int port;
    public byte itemID;
    public string ip, identifier;
    public NbtFile playerData;
    public Player(RakNetClient client, World w) : base(w)
    {
        Client = client;
        this.heartsLife = 20;
        this.inCreative = RakNetServer.Properties.gamemode;
        Define(EntityDataKey.IsSleeping, EntityDataType.Byte);
        Define(EntityDataKey.SleepPosition, EntityDataType.Pos);
    }

    public bool IsClientOf(RakNetClient client) => client.ClientID == Client.ClientID;


    public void SaveDat()
    {
        string playersFolder = Path.Combine("worlds", Client.Server.GameHandler.ServerWorld.World.LevelName, "players");

        playerData.RootTag.Clear();
        playerData.RootTag.Add(new NbtString("DisplayName", Username));
        playerData.RootTag.Add(new NbtByte("Health", heartsLife)); // TODO: Health
        playerData.RootTag.Add(new NbtByte("InCreative", inCreative ? (byte)1 : (byte)0));
        playerData.RootTag.Add(new NbtList("Position", new List<NbtFloat>() { new NbtFloat( Position.X),
                                                                           new NbtFloat( Position.Y),
                                                                           new NbtFloat( Position.Z) })
        );
        playerData.RootTag.Add(new NbtList("ViewAngle", new List<NbtFloat>() {new NbtFloat( ViewAngle.X),
                                                                           new NbtFloat( ViewAngle.Y),
                                                                           new NbtFloat( ViewAngle.Z) })
        );

        playerData.SaveToFile(Path.Combine(playersFolder, $"{Username}.dat"), NbtCompression.GZip);
    }

    public void LoadDat()
    {
        if (playerData.RootTag.Count > 1)
        {
            // TODO NULL Checks
            DisplayName = playerData.RootTag["DisplayName"].StringValue;
            heartsLife = playerData.RootTag["Health"].ByteValue;
            inCreative = playerData.RootTag["InCreative"].ByteValue == 1;
            NbtList pos = (NbtList)playerData.RootTag["Position"];
            Position = new Vector3(pos[0].FloatValue, pos[1].FloatValue, pos[2].FloatValue);
            NbtList angle = (NbtList)playerData.RootTag["ViewAngle"];
            ViewAngle = new Vector3(angle[0].FloatValue, angle[1].FloatValue, angle[2].FloatValue);
        }
        else
        {
            World world = Client.Server.GameHandler.ServerWorld.World;
            Position = new(world.SpawnX, world.SpawnY, world.SpawnZ);
        }
    }



    public void Send(ConnectedPacket packet, int reliability = ConnectedPacket.RELIABLE) =>
        Client.Send(packet, reliability);

    internal void heal(int healAmount)
    {
        throw new NotImplementedException();
    }
}
