namespace SpoongePE.Core.Network;

public enum MinecraftPacketType : byte
{
    // Who can send it?
    Unknown = 0x00, // Server (for split packets)
    LoginRequest = 0x82, // Client
    LoginResponse = 0x83, // Server
    Ready = 0x84, // Client
    Message = 0x85, // Both
    SetTime = 0x86, // Server
    StartGame = 0x87, // Server
    AddMob = 0x88, // Server
    AddPlayer = 0x89, // Server
    RemovePlayer = 0x8A, // Server
    AddEntity = 0x8C, // Server
    RemoveEntity = 0x8D, // Server
    AddItemEntity = 0x8E, // Server
    TakeItemEntity = 0x8F, // Server?
    MoveEntity = 0x90, // Unused
    MoveEntityPosRot = 0x93, // Server
    RotateHead = 0x94, // Server
    MovePlayer = 0x95, // Both
    PlaceBlock = 0x96, // Both
    RemoveBlock = 0x97, // Both
    UpdateBlock = 0x98, // Server?
    AddPainting = 0x99, // Server
    Explode = 0x9A, // Server
    LevelEvent = 0x9B, // Server
    TileEvent = 0x9C, // Server
    EntityEvent = 0x9D, // Server
    RequestChunk = 0x9E, // Client
    ChunkData = 0x9F, // Server
    PlayerEquipment = 0xA0, // Both
    PlayerArmorEquipment = 0xA1, // Both?
    Interact = 0xA2, // Client
    UseItem = 0xA3, // Client
    PlayerAction = 0xA4, // Client
    HurtArmor = 0xA6, // Server
    SetEntityData = 0xA7, // Server
    SetEntityMotion = 0xA8, // Server
    SetRiding = 0xA9, // Server
    SetHealth = 0xAA, // Server
    SetSpawnPosition = 0xAB, // Server
    Animate = 0xAC, // Server
    Respawn = 0xAD, // Client
    SendInventory = 0xAE, // Unused
    DropItem = 0xAF, // Client
    ContainerOpen = 0xB0, // Both?
    ContainerClose = 0xB1, // Both?
    ContainerSetSlot = 0xB2, // Server?
    ContainerSetData = 0xB3, // Server?
    ContainerSetContent = 0xB4, // Server?
    ContainerAck = 0xB5, // Both
    Chat = 0xB6, // Both
    AdventureSettings = 0xB7, // Server // ADVENTURE_SETTINGS_PACKET // SignUpdate??
    SetTileEntityData = 0xB8, // Server/Both?
    PlayerInput = 0xB9, // Client
    DATA_PACKET_0 = 0x80 // Unk
    /*
    Packets analogues in beta 1.7.3


    Packet1Login -> LoginResponse
    Packet2Handshake -> LoginRequest
    Packet3Chat -> Message
    Packet4UpdateTime -> SetTime
    Packet5PlayerInventory -> PlayerEquipment?
    Packet6SpawnPosition -> SetSpawnPosition
    Packet7UseEntity -> Interact?
    Packet8UpdateHealth -> SetHealth
    Packet9Respawn -> Respawn
    Packet10Flying -> not exist
    Packet11PlayerPosition -> not exist
    Packet12PlayerLook -> RotateHead
    Packet13PlayerLookMove -> MovePlayer
    Packet14BlockDig -> RemoveBlock
    Packet15Place -> PlaceBlock
    Packet16BlockItemSwitch(swing) -> PlayerEquipment???
    Packet17Sleep -> not exist
    Packet18Animation -> Animate?
    Packet19EntityAction -> Interact?
    Packet20NamedEntitySpawn -> AddPlayer
    Packet21PickupSpawn -> AddItemEntity
    Packet22Collect -> TakeItemEntity??
    Packet23VehicleSpawn -> AddEntityPacket
    Packet24MobSpawn -> AddMob
    Packet25EntityPainting -> AddPainting
    Packet28EntityVelocity -> SetEntityMotion
    Packet29DestroyEntity -> RemoveEntity
    Packet30Entity -> AddEntity
    Packet31RelEntityMove -> not exist
    Packet32EntityLook -> not exist
    Packet33RelEntityMoveLook -> MoveEntityPosRot
    Packet34EntityTeleport -> MoveEntityPosRot
    Packet38EntityStatus -> SetEntityData
    Packet39AttachEntity -> SetRiding?
    Packet40EntityMetadata -> SetEntityData
    Packet50PreChunk -> not exist
    Packet51MapChunk -> ChunkData
    Packet52MultiBlockChange -> not exist
    Packet53BlockChange -> UpdateBlock
    Packet54PlayNoteBlock -> not exist
    Packet60Explosion -> Explode
    Packet61DoorChange -> LevelEvent???
    Packet70Bed -> UpdateBlock/LevelEvent??
    Packet71Weather -> not exist
    Packet100OpenWindow -> ContainerOpen
    Packet101CloseWindow -> ContainerClose
    Packet102WindowClick -> ??
    Packet103SetSlot -> ContainerSetSlot
    Packet104WindowItems -> ContainerSetContent
    Packet105UpdateProgressbar -> SetTileEntityData?
    Packet106Transaction -> ContainerAck? xd
    Packet130UpdateSign -> SignUpdate? xddd
    Packet255KickDisconnect -> PlayerDisconnect
    */
}
