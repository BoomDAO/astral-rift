using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttributeModifier : MonoBehaviour
{
    public string entityId;
    public string groupId;
    public string worldId;
    public string newAttribute;

    public string GetKey()
    {
        var _worldId = worldId;
        if (string.IsNullOrEmpty(_worldId)) _worldId = Env.CanisterIds.WORLD;

        return $"{_worldId}{groupId}{entityId}";
    }

    public void Modify()
    {
        EntityUtil.SetCurrentAttribute(new DataTypes.Entity(worldId, groupId, entityId, null, newAttribute, null));

    }
}
