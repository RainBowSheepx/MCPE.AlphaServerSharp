using System;

namespace SpoongePE.Core.Game.ItemBase;

public class ItemInstance
{
    public int ItemID;
    public int Count;

    public int ItemMeta;
    // Here's stuff like Tile* and Item*, we don't use those yet.

    public ItemInstance()
    {

    }

    public ItemInstance(int itemID, int itemCount, int itemMeta)
    {
        ItemID = itemID;
        Count = itemCount;
        ItemMeta = itemMeta;
    }

    public Item getItem()
    {
        return Item.items[this.ItemID];
    }

    public bool useOn(Player player, World world, int x, int y, int z, int face)
    {
        return getItem().useOn(this, player, world, x, y, z, face);
    }
}
