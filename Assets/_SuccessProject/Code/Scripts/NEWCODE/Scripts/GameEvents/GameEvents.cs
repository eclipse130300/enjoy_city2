using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    //photon event-bytes
    public const byte PLAYER_IS_READY_PAINTBALL_LOBBY = 1;
    public const byte PLAYER_IS_READY_PAINTBALL_GAME = 2;
    public const byte START_CD_GAME_TIMER = 3;
    public const byte START_PAINTBALL_GAME = 4;
    public const byte HIT_RECIEVED = 5;
    public const byte UPDATE_SCORE = 6;
    public const byte PAINTBALL_GAME_FINISHED = 7;
    public const byte PLAYER_DEATH = 8;
    public const byte START_GAME_TIMER = 9;
    public const byte PLAYER_RESPAWNED = 10;
    public const byte PAINTBALL_FINISHED = 11;

    public const byte DIRTY_HIT_RECIEVED = 12;


    //city
    public const string ENTRY_POINT_ENTERED = "ENTRY_POINT_ENTERED";
    public const string ENTRY_POINT_EXIT = "ENTRY_POINT_EXIT";

    public const string INTERACTION_BUTTON_TAP = "INTERACTION_BUTTON_TAP";

    public const string LVL_CHANGED = "LVL_CHANGED";
    public const string EXP_CHANGED = "EXP_CHANGED";

    public const string CURRENCY_UPDATED = "CURRENCY_UPDATED";

    //items(room and clothes)
    public const string ITEM_PRESSED = "ITEM_PRESSED";

    public const string ITEM_OPERATION_DONE = "ITEM_OPERATION_DONE";
    public const string ITEM_PICKED = "ITEM_PICKED";

    public const string CLOTHES_CHANGED = "CLOTHES_CHANGED";

    public const string ITEM_VARIANT_CHANGED = "ITEM_VARIANT_CHANGED";


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
    public const string SUPER_SHOT_CD = "SUPER_SHOT_CD";

    public const string PAINTBALL_POWER_UP_PRESSED = "PAINTBALL_POWER_UP_PRESSED";
    public const string PAINTBALL_POWER_UP_CD = "PAINTBALL_POWER_UP_CD";

    public const string PAINTBALL_PLAYER_SPAWNED = "PAINTBALL_PLAYER_SPAWNED";
    public const string FIRING = "FIRING";
    public const string RELOADING = "RELOADING";
}
