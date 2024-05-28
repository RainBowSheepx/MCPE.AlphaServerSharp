using SpoongePE.Core.Game.entity.impl;
using SpoongePE.Core.NBT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.entity
{
    public class EntityList
    {
        private static Dictionary<string, Type> stringToClassMapping = new Dictionary<string, Type>();
        private static Dictionary<Type, string> classToStringMapping = new Dictionary<Type, string>();
        private static Dictionary<int, Type> IDtoClassMapping = new Dictionary<int, Type>();
        private static Dictionary<Type, int> classToIDMapping = new Dictionary<Type, int>();

        private static void AddMapping(Type var0, string var1, int var2)
        {
            stringToClassMapping[var1] = var0;
            classToStringMapping[var0] = var1;
            IDtoClassMapping[var2] = var0;
            classToIDMapping[var0] = var2;
        }

        public static Entity CreateEntityInWorld(string var0, World var1)
        {
            Entity var2 = null;

            try
            {
                Type var3 = stringToClassMapping[var0];
                if (var3 != null)
                {
                    var2 = (Entity)Activator.CreateInstance(var3, var1);
                }
            }
            catch (Exception var4)
            {
                Console.WriteLine(var4.Message);
            }

            return var2;
        }

        public static Entity CreateEntityFromNBT(NbtCompound var0, World var1)
        {
            Entity var2 = null;

            try
            {
                Type var3 = stringToClassMapping[var0["id"].StringValue];
                if (var3 != null)
                {
                    var2 = (Entity)Activator.CreateInstance(var3, var1);
                }
            }
            catch (Exception var4)
            {
                Console.WriteLine(var4.Message);
            }

            if (var2 != null)
            {
                var2.readFromNBT(var0);
            }
            else
            {
                Console.WriteLine("Skipping Entity with id " + var0["id"].StringValue);
            }

            return var2;
        }

        public static Entity CreateEntity(int var0, World var1)
        {
            Entity var2 = null;

            try
            {
                Type var3 = IDtoClassMapping[var0];
                if (var3 != null)
                {
                    var2 = (Entity)Activator.CreateInstance(var3, var1);
                }
            }
            catch (Exception var4)
            {
                Console.WriteLine(var4.Message);
            }

            if (var2 == null)
            {
                Console.WriteLine("Skipping Entity with id " + var0);
            }

            return var2;
        }

        public static int GetEntityID(Entity var0)
        {
            return classToIDMapping[var0.GetType()];
        }

        public static string GetEntityString(Entity var0)
        {
            return classToStringMapping[var0.GetType()];
        }

        static EntityList()
        {

            //AddMapping(typeof(EntityPlayer), "Player", 1);

            AddMapping(typeof(EntityChicken), "Chicken", 10);
            AddMapping(typeof(EntityCow), "Cow", 11);
            AddMapping(typeof(EntityPig), "Pig", 12);
            AddMapping(typeof(EntitySheep), "Sheep", 13);

            AddMapping(typeof(EntityZombie), "Zombie", 32);
            AddMapping(typeof(EntityCreeper), "Creeper", 33);
            AddMapping(typeof(EntitySkeleton), "Skeleton", 34);
            AddMapping(typeof(EntitySpider), "Spider", 35);
            AddMapping(typeof(EntityPigZombie), "PigZombie", 36);

            AddMapping(typeof(EntityArrow), "Arrow", 80);
            AddMapping(typeof(EntitySnowball), "Snowball", 81);
            //AddMapping EntityEgg here
            AddMapping(typeof(EntityPainting), "Painting", 83);
            AddMapping(typeof(EntityMinecart), "Minecart", 84);
            AddMapping(typeof(EntityTNTPrimed), "PrimedTnt", 20);

            AddMapping(typeof(EntityItem), "Item", 64);
            AddMapping(typeof(EntityFallingSand), "FallingSand", 66);


        }
    }
}
