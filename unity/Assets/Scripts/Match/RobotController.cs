﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RobotController : MonoBehaviour
{
    public AnimatorHelper animatorHelper;
    public GameObject defaultModel;
    public MeshRenderer healthMeshRenderer;
    public MeshRenderer attackMeshRenderer;
    public TextMesh healthLabel;
    public TextMesh attackLabel;
    public GameObject[] robotModels;

    internal short id { get; private set; }
    internal bool isSpawned;
    internal bool isOpponent;
    internal List<SpriteRenderer> currentEvents = new List<SpriteRenderer>();
    internal List<Command> commands = new List<Command>();

    public void LoadModel(string n, short i)
    {
        id = i;
        GameObject model = new List<GameObject>(robotModels).Find(g => g.name.Equals(n));
        if (model == null) model = defaultModel;
        GameObject baseModel = Instantiate(model, animatorHelper.transform);
    }
    
    /***********************************
     * Robot Model Before Turn Methods *
     ***********************************/

    internal void AddRobotCommand(Command cmd, UnityAction<Command, short, bool> callback)
    {
        int num = GetNumCommandType(cmd.commandId);
        if (num < Command.limit[cmd.commandId])
        {
            commands.Add(cmd);
            callback(cmd, id, isOpponent);
        }
    }

    private int GetNumCommandType(byte t)
    {
        return commands.Count(c => c.commandId == t);
    }

    internal void ShowMenuOptions(ButtonContainerController m)
    {
        if (!isSpawned && commands.Count == 0)
        {
            Command.TYPES.ToList().ForEach(t => m.GetByName(Command.GetDisplay[t]).SetActive(t == Command.SPAWN_COMMAND_ID));
        }
        else
        {
            Command.TYPES.ToList().ForEach(t =>
            {
                int num = GetNumCommandType(t);
                bool active = num < Command.limit[t] && t!=Command.SPAWN_COMMAND_ID;
                MenuItemController item = m.GetByName(Command.GetDisplay[t]);
                item.SetActive(active);
            });
        }
    }

    internal void AddRobotCommand(string name, byte dir, UnityAction<Command, short, bool> callback)
    {
        if (name.Equals(Command.GetDisplay[Command.SPAWN_COMMAND_ID]))
        {
            AddRobotCommand(new Command(dir, Command.SPAWN_COMMAND_ID), callback);
        }
        else if (name.Equals(Command.GetDisplay[Command.MOVE_COMMAND_ID]))
        {
            AddRobotCommand(new Command(dir, Command.MOVE_COMMAND_ID), callback);
        }
        else if (name.Equals(Command.GetDisplay[Command.ATTACK_COMMAND_ID]))
        {
            AddRobotCommand(new Command(dir, Command.ATTACK_COMMAND_ID), callback);
        }
    }

    /********************************
     * Robot UI During Turn Methods *
     ********************************/

    public void displaySpawnRequest(UnityAction robotCallback)
    {
        Debug.Log(id + " is displaying a requested spawn");
        animatorHelper.Animate("SpawnRequest", robotCallback);
    }

    public void displaySpawn(UnityAction robotCallback)
    {
        isSpawned = true;
        animatorHelper.Animate("Spawn", robotCallback);
    }

    public void displayMove(Vector2Int v, BoardController boardController, UnityAction robotCallback)
    {
        animatorHelper.Animate("Move" + getDir(v), () => {
            boardController.UnplaceRobot(this);
            boardController.PlaceRobot(this, v.x, v.y);
            robotCallback();
        });
    }

    public void displayMoveRequest(Vector2Int v, UnityAction robotCallback)
    {
        animatorHelper.Animate("MoveRequest" + getDir(v), robotCallback);
    }

    internal void DisplayBlocked(UnityAction callback)
    {
        animatorHelper.Animate("Blocked", callback);
    }

    public void displayAttack(Vector2Int v, UnityAction robotCallback)
    {
        animatorHelper.Animate("Attack" + getDir(v), robotCallback);
    }

    public void displayDamage(short h, UnityAction robotCallback)
    {
        displayHealth(h);
        animatorHelper.Animate("Damage", robotCallback);
    }

    public void displayDeath(short health, UnityAction<RobotController> robotCallback)
    {
        animatorHelper.Animate("Death", () => {
            displayHealth(health);
            gameObject.SetActive(false);
            robotCallback(this);
        });
    }

    public void displayHealth(short health)
    {
        healthLabel.text = health.ToString();
    }

    public void displayAttack(short attack)
    {
        attackLabel.text = attack.ToString();
    }

    public short GetHealth()
    {
        return short.Parse(healthLabel.text);
    }

    public short GetAttack()
    {
        return short.Parse(attackLabel.text);
    }

    private string getDir(Vector2Int v)
    {
        string dir = "Reset";
        if (v - new Vector2Int((int)transform.position.x, (int)transform.position.y) == Vector2Int.left) dir = "Left";
        if (v - new Vector2Int((int)transform.position.x, (int)transform.position.y) == Vector2Int.right) dir = "Right";
        if (v - new Vector2Int((int)transform.position.x, (int)transform.position.y) == Vector2Int.up) dir = "Up";
        if (v - new Vector2Int((int)transform.position.x, (int)transform.position.y) == Vector2Int.down) dir = "Down";
        return dir;
    }

    public void clearEvents()
    {
        currentEvents.ForEach(i => Destroy(i.gameObject));
        currentEvents.Clear();
    }
}
