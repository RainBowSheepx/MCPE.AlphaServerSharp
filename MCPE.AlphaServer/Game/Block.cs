using MCPE.AlphaServer.Game.BlockBase;
using MCPE.AlphaServer.Game.BlockBase.impl;
using MCPE.AlphaServer.Game.material;
using MCPE.AlphaServer.Game.utils.random;
using MCPE.AlphaServer.Network;
using MCPE.AlphaServer.Utils;
using System;

namespace MCPE.AlphaServer.Game;

public abstract class Block {

    public static Block[] blocks = new Block[256];
    public static bool[] shouldTick = new bool[256];
    public static int[] lightOpacity = new int[256];

    public static SolidBlock stone = new SolidBlock(1, Material.stone).setBlockName("Stone"); //love old mcje code btw <<<
    public static SolidBlock grass = new SolidBlock(2, Material.dirt).setBlockName("Grass");
    public static SolidBlock dirt = new SolidBlock(3, Material.dirt).setBlockName("Dirt");
    public static SolidBlock cobblestone = new SolidBlock(4, Material.stone).setBlockName("Cobblestone");
    public static SolidBlock wood = new SolidBlock(5, Material.wood).setBlockName("Wood");
    public static SolidBlock bedrock = new SolidBlock(7, Material.stone).setBlockName("Bedrock");
    public static LiquidStaticBlock waterFlowing = new LiquidStaticBlock(8, Material.water);
    public static LiquidStaticBlock waterStill = new LiquidStaticBlock(9, Material.water);
    public static LiquidStaticBlock lavaFlowing = new LiquidStaticBlock(10, Material.lava);
    public static LiquidStaticBlock lavaStill = new LiquidStaticBlock(11, Material.lava);
    public static SolidBlock sand = new SolidBlock(12, Material.sand);
    public static SolidBlock gravel = new SolidBlock(13, Material.sand);
    public static SolidBlock oreGold = new SolidBlock(14, Material.stone);
    public static SolidBlock oreIron = new SolidBlock(15, Material.stone);
    public static SolidBlock oreCoal = new SolidBlock(16, Material.stone);
    public static SolidBlock log = new SolidBlock(17, Material.wood);
    public static SolidBlock leaves = new SolidBlock(18, Material.leaves);
    public static SolidBlock glass = new SolidBlock(20, Material.glass);
    public static SolidBlock lapisOre = new SolidBlock(21, Material.stone);
    public static SolidBlock sandStone = new SolidBlock(24, Material.stone);
    public static PlantBlock yellowFlowerBlock = new PlantBlock(37, Material.plant);
    public static PlantBlock rose = new PlantBlock(38, Material.plant);
    public static PlantBlock brownMushroom = new PlantBlock(39, Material.plant);
    public static PlantBlock redMushroom = new PlantBlock(40, Material.plant);
    public static SolidBlock goldBlock = new SolidBlock(41, Material.metal);
    public static SolidBlock ironBlock = new SolidBlock(42, Material.metal);
    public static SolidBlock fullStoneSlab = new SolidBlock(43, Material.stone);
    public static SolidBlock stoneSlab = new SolidBlock(44, Material.stone);
    public static SolidBlock brick = new SolidBlock(45, Material.stone);
    public static SolidBlock tnt = new SolidBlock(46, Material.stone);
    public static SolidBlock obsidian = new SolidBlock(49, Material.stone);
    public static DecorationBlock torch = new DecorationBlock(50, Material.decoration);
    public static SolidBlock woodStairs = new SolidBlock(53, Material.wood);
    public static SolidBlock diamondOre = new SolidBlock(56, Material.stone);
    public static SolidBlock diamondBlock = new SolidBlock(57, Material.metal);
    public static SolidBlock farmland = new SolidBlock(60, Material.dirt);
    public static DoorBlock woodenDoor = new DoorBlock(64, Material.wood);//
    public static DecorationBlock ladder = new DecorationBlock(65, Material.wood); //
    public static StairsBlock stoneStairs = new StairsBlock(67, Material.stone);
    public static DoorBlock ironDoor = new DoorBlock(71, Material.metal);
    public static SolidBlock redstoneOre = new SolidBlock(73, Material.stone);
    public static SolidBlock glowingRedstoneOre = new SolidBlock(74, Material.stone);
    public static DecorationBlock snowLayer = new DecorationBlock(78, Material.decoration);
    public static SolidBlock ice = new SolidBlock(79, Material.ice);
    public static SolidBlock cactus = new SolidBlock(81, Material.cactus);
    public static SolidBlock clay = new SolidBlock(82, Material.clay);
    public static PlantBlock reeds = new PlantBlock(83, Material.plant);
    public static SolidBlock invisibleBedrock = new SolidBlock(95,Material.stone); //TODO destructible/indestructible
    public static WoolBlock wool = new WoolBlock(35, -1);
    public static WoolBlock wool_f = new WoolBlock(101, 0xf); //using ids instead of meta =/
    public static WoolBlock wool_e = new WoolBlock(102, 0xe);
    public static WoolBlock wool_d = new WoolBlock(103, 0xd);
    public static WoolBlock wool_c = new WoolBlock(104, 0xc);
    public static WoolBlock wool_b = new WoolBlock(105, 0xb);
    public static WoolBlock wool_a = new WoolBlock(106, 0xa);
    public static WoolBlock wool_9 = new WoolBlock(107, 0x9);
    public static WoolBlock wool_8 = new WoolBlock(108, 0x8);
    public static WoolBlock wool_7 = new WoolBlock(109, 0x7);
    public static WoolBlock wool_6 = new WoolBlock(110, 0x6);
    public static WoolBlock wool_5 = new WoolBlock(111, 0x5);
    public static WoolBlock wool_4 = new WoolBlock(112, 0x4);
    public static WoolBlock wool_3 = new WoolBlock(113, 0x3);
    public static WoolBlock wool_2 = new WoolBlock(114, 0x2);
    public static WoolBlock wool_1 = new WoolBlock(115, 0x1);
    public static SolidBlock updateGame = new SolidBlock(248, Material.stone);
    public static SolidBlock updateGame2 = new SolidBlock(249, Material.stone);
    public static DecorationBlock fire = new DecorationBlock(51, Material.fire);


    public void onNeighborBlockChanged(World world, int x, int y, int z, int meta) { }
    public void onBlockRemoved(World world, int x, int y, int z)
    {
        world.removeBlock(x, y, z);
        UpdateBlockPacket pk = new UpdateBlockPacket();
        pk.X = x;
        pk.Y = (byte)y;
        pk.Z = z;
        pk.Block = 0;
        pk.Meta = 0;
        world.broadcastPacket(pk);
    }
    public void onBlockRemovedByPlayer(World world, int x, int y, int z, Player player)
    {
        world.removeBlock(x, y, z);
    }

    public void tick(World world, int x, int y, int z, BedrockRandom random)
    {

    }

    public void onBlockAdded(World world, int x, int y, int z)
    {

    }
    public Block setBlockName(string name)
    {
        this.name = name;
        return this;
    }
    public bool canSurvive(World world, int x, int y, int z)
    {
        return true;
    }

    public void onBlockPlacedByPlayer(World world, int x, int y, int z, int face, Player player)
    {
        world.placeBlockAndNotifyNearby(x, y, z, (byte)this.blockID);
    }

    public static void init() { }

    public Block(int id, Material m)
    {
        this.blockID = id;
        this.material = m;
        if (Block.blocks[id] != null && Block.blocks[id].GetType() == typeof(Block)){
            Logger.Error("ID " + id + " is occupied already!");
        }
        else {
            Block.blocks[id] = this; 
        }

    }

    public int blockID;
    public String name = "";
    public Material material;
    public bool isSolid = true; //isRenderSolid method in 0.1.3
    public bool isOpaque = true;
}
