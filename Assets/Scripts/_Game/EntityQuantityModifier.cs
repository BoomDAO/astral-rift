using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityQuantityModifier : MonoBehaviour
{
    public enum EntityQuantityMod
    {
        Set,
        Add,
        Sub
    }
    public string entityId;
    public string groupId;
    public string worldId;
    public long quantity;
    public EntityQuantityMod quantityMod;

    public string GetKey()
    {
        var _worldId = worldId;
        if (string.IsNullOrEmpty(_worldId)) _worldId = Env.CanisterIds.WORLD;

        return $"{_worldId}{groupId}{entityId}";
    }

    public void Modify()
    {
        if (quantityMod == EntityQuantityMod.Set)
        {
            EntityUtil.SetCurrentQuantity(new DataTypes.Entity(worldId, groupId, entityId, quantity, null, null));
        }
        else if (quantityMod == EntityQuantityMod.Add)
        {
            EntityUtil.IncrementCurrentQuantity(new DataTypes.Entity(worldId, groupId, entityId, quantity, null, null));
        }
        else
        {
            EntityUtil.DecrementCurrentQuantity(new DataTypes.Entity(worldId, groupId, entityId, quantity, null, null));
        }
    }
}
