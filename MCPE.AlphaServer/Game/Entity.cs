using System.Collections.Generic;

namespace MCPE.AlphaServer.Game;

public abstract class Entity {
    private static int LastEntityID = 1;

    public int EntityID;
    
    public EntityData EntityData;
    public float posX, posY, posZ;
    public float yaw, pitch;
  
    public World world;
    public Entity() {
        EntityID = LastEntityID++;
     
        EntityData = new EntityData();
        this.posX = 64;
        this.posY = 128;
        this.posZ = 64;
        this.yaw = 0;
        this.pitch = 0;
      
        Define(EntityDataKey.Flags, EntityDataType.Byte);
        Define(EntityDataKey.Air, EntityDataType.Short);
    }

    public void Define(EntityDataKey id, EntityDataType dataType) => EntityData.Define(id, dataType);
    public void Set(EntityDataKey id, object value) => EntityData.Set(id, value);
    public T Get<T>(EntityDataKey id) => EntityData.Get<T>(id);
}
