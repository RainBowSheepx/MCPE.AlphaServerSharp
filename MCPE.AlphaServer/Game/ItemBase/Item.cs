using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.ItemBase.impl;
using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase
{
    public class Item
    {
        // Source
        // net/minecraft/src/Item.java
        public static Item[] itemsList = new Item[512];

        public int itemID;
        public int shiftedIndex;
        protected int maxStackSize = 64;
        private int maxDamage = 0;
        protected bool hasSubtypes = false;
        private Item containerItem = null;
        public Item(int itemID)
        {
            this.itemID = itemID + 256;
            Item.itemsList[this.itemID] = this;
        }

        public bool onItemUse(ItemStack item, Player player, World world, int x, int y, int z, int side) => false;





        public Item setMaxStackSize(int var1)
        {
            this.maxStackSize = var1;
            return this;
        }



        public float getStrVsBlock(ItemStack var1, Block var2) => 1.0F;


        public ItemStack onItemRightClick(ItemStack var1, World var2, Player var3) => var1;


        public int getItemStackLimit() => this.maxStackSize;


        public int getPlacedBlockMetadata(int var1) => 0;


        public bool getHasSubtypes() => this.hasSubtypes;


        protected Item setHasSubtypes(bool var1)
        {
            this.hasSubtypes = var1;
            return this;
        }

        public int getMaxDamage() => this.maxDamage;

        protected Item setMaxDamage(int var1)
        {
            this.maxDamage = var1;
            return this;
        }

        public bool isDamagable() => this.maxDamage > 0 && !this.hasSubtypes;


        //  public bool hitEntity(ItemStack var1, EntityLiving var2, EntityLiving var3) => false;


        public bool onBlockDestroyed(ItemStack var1, int var2, int var3, int var4, int var5, Player var6) => false;


        public int getDamageVsEntity(Entity var1) => 1;


        public bool canHarvestBlock(Block var1) => false;


        //        public void saddleEntity(ItemStack var1, EntityLiving var2)



        public bool shouldRotateAroundWhenRendering() => false;

        public Item setContainerItem(Item var1)
        {
            if (this.maxStackSize > 1)
            {
                throw new ArgumentException("Max stack size must be 1 for items with crafting results");
            }
            else
            {
                this.containerItem = var1;
                return this;
            }
        }

        public Item getContainerItem() => this.containerItem;


        public bool hasContainerItem() => this.containerItem != null;


        public int getColorFromDamage(int var1) => 16777215;


        public void onUpdate(ItemStack var1, World var2, Entity var3, int var4, bool var5) { }


        public void onCreated(ItemStack var1, World var2, Player var3) { }

        public static Item shovelSteel = new ItemSpade(0, EnumToolMaterial.IRON);
        public static Item pickaxeSteel = new ItemPickaxe(1, EnumToolMaterial.IRON);
        public static Item axeSteel = new ItemAxe(2, EnumToolMaterial.IRON);
        public static Item flintAndSteel = new ItemFlintAndSteel(3);
        public static Item appleRed = new ItemFood(4, 4);
        public static Item bow = new ItemBow(5);
        public static Item arrow = new Item(6);
        public static Item coal = new Item(7);
        public static Item diamond = new Item(8);
        public static Item ingotIron = new Item(9);
        public static Item ingotGold = new Item(10);
        public static Item swordSteel = new ItemSword(11, EnumToolMaterial.IRON);
        public static Item swordWood = new ItemSword(12, EnumToolMaterial.WOOD);
        public static Item shovelWood = new ItemSpade(13, EnumToolMaterial.WOOD);
        public static Item pickaxeWood = new ItemPickaxe(14, EnumToolMaterial.WOOD);
        public static Item axeWood = new ItemAxe(15, EnumToolMaterial.WOOD);
        public static Item swordStone = new ItemSword(16, EnumToolMaterial.STONE);
        public static Item shovelStone = new ItemSpade(17, EnumToolMaterial.STONE);
        public static Item pickaxeStone = new ItemPickaxe(18, EnumToolMaterial.STONE);
        public static Item axeStone = new ItemAxe(19, EnumToolMaterial.STONE);
        public static Item swordDiamond = new ItemSword(20, EnumToolMaterial.EMERALD);
        public static Item shovelDiamond = new ItemSpade(21, EnumToolMaterial.EMERALD);
        public static Item pickaxeDiamond = new ItemPickaxe(22, EnumToolMaterial.EMERALD);
        public static Item axeDiamond = new ItemAxe(23, EnumToolMaterial.EMERALD);
        public static Item stick = new Item(24);
        public static Item bowlEmpty = new Item(25);
        public static Item bowlSoup = new ItemSoup(26, 10);
        public static Item swordGold = new ItemSword(27, EnumToolMaterial.GOLD);
        public static Item shovelGold = new ItemSpade(28, EnumToolMaterial.GOLD);
        public static Item pickaxeGold = new ItemPickaxe(29, EnumToolMaterial.GOLD);
        public static Item axeGold = new ItemAxe(30, EnumToolMaterial.GOLD);
        public static Item silk = new Item(31);
        public static Item feather = new Item(32);
        public static Item gunpowder = new Item(33);
        public static Item hoeWood = new ItemHoe(34, EnumToolMaterial.WOOD);
        public static Item hoeStone = new ItemHoe(35, EnumToolMaterial.STONE);
        public static Item hoeSteel = new ItemHoe(36, EnumToolMaterial.IRON);
        public static Item hoeDiamond = new ItemHoe(37, EnumToolMaterial.EMERALD);
        public static Item hoeGold = new ItemHoe(38, EnumToolMaterial.GOLD);
        public static Item seeds = new ItemSeeds(39, Block.wheatBlock.blockID);
        public static Item wheat = new Item(40);
        public static Item bread = new ItemFood(41, 5);
        public static Item helmetLeather = new ItemArmor(42, 0, 0, 0);
        public static Item plateLeather = new ItemArmor(43, 0, 0, 1);
        public static Item legsLeather = new ItemArmor(44, 0, 0, 2);
        public static Item bootsLeather = new ItemArmor(45, 0, 0, 3);
        public static Item helmetChain = new ItemArmor(46, 1, 1, 0);
        public static Item plateChain = new ItemArmor(47, 1, 1, 1);
        public static Item legsChain = new ItemArmor(48, 1, 1, 2);
        public static Item bootsChain = new ItemArmor(49, 1, 1, 3);
        public static Item helmetSteel = new ItemArmor(50, 2, 2, 0);
        public static Item plateSteel = new ItemArmor(51, 2, 2, 1);
        public static Item legsSteel = new ItemArmor(52, 2, 2, 2);
        public static Item bootsSteel = new ItemArmor(53, 2, 2, 3);
        public static Item helmetDiamond = new ItemArmor(54, 3, 3, 0);
        public static Item plateDiamond = new ItemArmor(55, 3, 3, 1);
        public static Item legsDiamond = new ItemArmor(56, 3, 3, 2);
        public static Item bootsDiamond = new ItemArmor(57, 3, 3, 3);
        public static Item helmetGold = new ItemArmor(58, 1, 4, 0);
        public static Item plateGold = new ItemArmor(59, 1, 4, 1);
        public static Item legsGold = new ItemArmor(60, 1, 4, 2);
        public static Item bootsGold = new ItemArmor(61, 1, 4, 3);
        public static Item flint = new Item(62);
        public static Item porkRaw = (new ItemFood(63, 3));
        public static Item porkCooked = (new ItemFood(64, 8));
        public static Item painting = (new ItemPainting(65));
        public static Item appleGold = (new ItemFood(66, 42));
        public static Item sign = (new ItemSign(67));
        public static Item doorWood = (new ItemDoor(68, Material.wood));
        public static Item bucketEmpty = (new ItemBucket(69, 0));
        public static Item bucketWater = (new ItemBucket(70, Block.waterFlowing.blockID));
        public static Item bucketLava = (new ItemBucket(71, Block.lavaFlowing.blockID));
        public static Item minecartEmpty = (new ItemMinecart(72));
        public static Item saddle = (new ItemSaddle(73));
        public static Item doorSteel = (new ItemDoor(74, Material.metal));

        public static Item snowball = (new ItemSnowball(76));

        public static Item leather = (new Item(78));
        public static Item bucketMilk = (new ItemBucket(79, -1));
        public static Item brick = (new Item(80));
        public static Item clay = (new Item(81));
        public static Item reed = (new ItemReed(82, Block.reeds));
        public static Item paper = (new Item(83));
        public static Item book = (new Item(84));
        public static Item slimeBall = (new Item(85));

        public static Item egg = (new ItemEgg(88));
        public static Item compass = (new Item(89));

        public static Item pocketSundial = (new Item(91));
        public static Item lightStoneDust = (new Item(92));
        public static Item dyePowder = (new ItemDye(95));
        public static Item bone = (new Item(96));
        public static Item sugar = (new Item(97));
        public static Item cake = (new ItemReed(98, Block.cake));
        public static Item bed = (new ItemBed(99));



        public static ItemShears shears = (new ItemShears(103));

        // Items from 0.8.1
        public static Item melon = new ItemFood(104, 1);
        public static Item pumpkinSeeds = new ItemSeeds(105, Block.pumpkinStem.blockID);
        public static Item melonSeeds = new ItemSeeds(106, Block.melonStem.blockID);
        public static Item rawBeef = new ItemFood(107, 3);
        public static Item cookedBeef = new ItemFood(108, 8);
        public static Item rawChicken = new ItemFood(109, 3);
        public static Item cookedChicken = new ItemFood(110, 8);

        public static Item spawnEgg = new ItemSpawnEgg(127);

        public static Item carrot = new ItemSeeds(135, Block.carrot.blockID);
        public static Item potato = new ItemSeeds(136, Block.potato.blockID);
        public static Item bakedPotato = new ItemFood(137, 8);

        public static Item netherBrick = new Item(149);
        public static Item quartz = new Item(150);

        public static Item camera = new ItemCamera(200);
        public static Item beetroot = new ItemFood(201, 3);
        public static Item beetrootSeeds = new ItemSeeds(202, Block.beetroot.blockID);
        public static Item beetrootSoup = new ItemSoup(203, 8);
    }
}
