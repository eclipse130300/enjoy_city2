using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    //photon event-bytes
    public const byte NEWPLAYER_CONNECTED_EVENT = 1;


    //city
    public const string ENTRY_POINT_ENTERED = "ENTRY_POINT_ENTERED";
    public const string ENTRY_POINT_EXIT = "ENTRY_POINT_EXIT";

    public const string INTERACTION_BUTTON_TAP = "INTERACTION_BUTTON_TAP";

    public const string LVL_CHANGED = "LVL_CHANGED";
    public const string EXP_CHANGED = "EXP_CHANGED";

    //items(room and clothes)
    public const string ITEM_PRESSED = "ITEM_PRESSED";

    public const string ITEM_OPERATION_DONE = "ITEM_OPERATION_DONE";
    public const string ITEM_PICKED = "ITEM_PICKED";

    public const string CLOTHES_CHANGED = "CLOTHES_CHANGED";

    public const string ITEM_VARIANT_CHANGED = "ITEM_VARIANT_CHANGED";
    public const string REFRESH_SANDBOX_UI = "REFRESH_SANDBOX_UI";


    public const string INVENTORY_GAME_MODE_CHANGED = "INVENTORY_GAME_MODE_CHANGED";
    public const string INVENTORY_BODY_PART_CHANGED = "INVENTORY_BODY_PART_CHANGED";
    public const string FURNITURE_CHANGED = "FURNITURE_CHANGED";


    public const string CLOTHES_CONFIG_LOADED = "CLOTHES_CONFIG_LOADED";
    public const string ROOM_CONFIG_LOADED = "ROOM_CONFIG_LOADED";

    public const string ITEM_BOUGHT = "ITEM_BOUGHT";
    public const string ROOM_ITEM_BOUGHT = "ROOM_ITEM_BOUGHT";
    public const string ROOM_ITEM_PICKED = "ROOM_ITEM_PICKED";

    //paintBall
    public const string AUTO_SHOOT = "AUTO_SHOOT";
    public const string AMMO_UPDATED = "AMMO_UPDATED";
    public const string RELOAD_PRESSED = "RELOAD_PRESSED";
    public const string SUPER_SHOT_PRESSED = "SUPER_SHOT_PRESSED";
    public const string PAINTBALL_POWER_UP_PRESSED = "PAINTBALL_POWER_UP_PRESSED";
}
