using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectLayer
{
    public static int Default { get; private set; }
    public static int Player { get; private set; }
    public static int Projectile { get; private set; }
    public static int Target { get; private set; }
    public static int Interaction { get; private set; }
    public static int Pickup { get; private set; }

    static ObjectLayer()
    {
        Default = LayerMask.NameToLayer("Default");
        Player = LayerMask.NameToLayer("player");
        Projectile = LayerMask.NameToLayer("Projectile");
        Target = LayerMask.NameToLayer("Target");
        Interaction = LayerMask.NameToLayer("Interaction");
        Pickup = LayerMask.NameToLayer("Pickup");
    }
}

public static class ObjectLayerMask
{
    public static LayerMask Default { get; private set; }
    public static LayerMask Player { get; private set; }
    public static LayerMask Target { get; private set; }
    public static LayerMask BlockingProjectiles { get; private set; }
    public static LayerMask Environment { get; private set; }

    static ObjectLayerMask()
    {
        Default = 1 << ObjectLayer.Default;
        Player = 1 << ObjectLayer.Player;
        Target = 1 << ObjectLayer.Target;

        Environment = Default;
        BlockingProjectiles = Default | Player | Target;
    }
}
