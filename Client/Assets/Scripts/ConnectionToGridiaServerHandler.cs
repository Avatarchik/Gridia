﻿using Gridia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serving;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionToGridiaServerHandler : ConnectionToServerHandler
{
    private GridiaGame _game;
    private HashSet<Vector3> _sectorsRequested = new HashSet<Vector3>();
    private HashSet<int> _creaturesRequested = new HashSet<int>();

    public ConnectionToGridiaServerHandler(GridiaGame game, String host, int port)
        : base(host, port, new GridiaProtocols(), BoundDest.SERVER)
    {
        _game = game;
    }

    protected override void OnConnectionSettled()
    {
        MonoBehaviour.print("Connection settled!");
    }

    protected override void Run()
    {
        try
        {
            base.Run();
        }
        catch (Exception ex) 
        {
            Debug.LogError(ex);
        }
    }

    protected override void HandleData(int type, JObject data)
    {
        //TODO:
        //Unity doesn't compile this : data["id"].Value<int>()
        //So, I have to use: (int)data["id"]
        //Why?
        //MonoBehaviour.print((GridiaProtocols.Clientbound)type + " " + data);
        switch ((GridiaProtocols.Clientbound)type)
        {
            case GridiaProtocols.Clientbound.Initialize:
                GridiaConstants.SIZE = (int)data["size"];
                GridiaConstants.DEPTH = (int)data["depth"];
                GridiaConstants.SECTOR_SIZE = (int)data["sectorSize"];
                GridiaDriver.connectedWaitHandle.Set(); // :(
                GridiaDriver.gameInitWaitHandle.WaitOne();
                break;
            case GridiaProtocols.Clientbound.AddCreature:
                _game.CreateCreature((int)data["id"], (int)data["image"], (int)data["loc"]["x"], (int)data["loc"]["y"], (int)data["loc"]["z"]);
                break;
            case GridiaProtocols.Clientbound.MoveCreature:
                _game.MoveCreature((int)data["id"], (int)data["loc"]["x"], (int)data["loc"]["y"], (int)data["loc"]["z"]);
                break;
            case GridiaProtocols.Clientbound.RemoveCreature:
                _game.RemoveCreature((int)data["id"]);
                break;
            case GridiaProtocols.Clientbound.Chat:
                GridiaDriver.chatArea += data["msg"] + "\n";
                GridiaDriver.scrollPosition = new Vector2(0, int.MaxValue);
                break;
            case GridiaProtocols.Clientbound.SetFocus:
                var id = (int)data["id"];
                MonoBehaviour.print("focus id: " + id);
                _game.view.FocusId = id;
                _game.stateMachine = new StateMachine();
                _game.stateMachine.SetState(new PlayerMovementState(8));
                break;
            case GridiaProtocols.Clientbound.TileUpdate:
                var item = (int) data["item"];
                var quantity = (int) data["quantity"];
                var x = (int)data["loc"]["x"];
                var y = (int)data["loc"]["y"];
                var z = (int)data["loc"]["z"];
                _game.tileMap.SetItem(Locator.Get<ContentManager>().GetItem(item).GetInstance(quantity), x, y, z);
                break;
            case GridiaProtocols.Clientbound.Inventory:
                var backToJson = JsonConvert.SerializeObject(data["inv"]);
                Locator.Get<InventoryGUI>().Inventory = JsonConvert.DeserializeObject<List<ItemInstance>>(backToJson, new ItemInstanceConverter());
                break;
        }
    }

    protected override void HandleData(int type, JavaBinaryReader data)
    {
        switch ((GridiaProtocols.Clientbound)type)
        {
            case GridiaProtocols.Clientbound.SectorData:
                var sx = data.ReadInt32();
                var sy = data.ReadInt32();
                var sz = data.ReadInt32();
                var sectorSize = _game.tileMap.SectorSize;
                var tiles = new Tile[sectorSize, sectorSize];
                var cm = Locator.Get<ContentManager>();

                for (int x = 0; x < sectorSize; x++)
                {
                    for (int y = 0; y < sectorSize; y++)
                    {
                        var floor = data.ReadInt16();
                        var itemType = data.ReadInt16();
                        var tile = new Tile();
                        tile.Floor = floor;
                        tile.Item = new ItemInstance(cm.GetItem(itemType));
                        tiles[x, y] = tile;
                    }
                }
                _game.tileMap.SetSector(new Sector(tiles), sx, sy, sz);

                var numCreatures = data.ReadInt32();
                for (int i = 0; i < numCreatures; i++)
                {
                    var id = data.ReadInt16();
                    var image = data.ReadInt16();
                    var x = data.ReadInt16();
                    var y = data.ReadInt16();
                    var z = data.ReadInt16();
                    _game.CreateCreature(id, image, x, y, z);
                }
                break;
        }
    }

    public void PlayerMove(Vector3 loc)
    {
        Message message = new JsonMessageBuilder()
            .Protocol(Outbound(GridiaProtocols.Serverbound.PlayerMove))
            .Set("loc", new { x = loc.x, y = loc.y, z = loc.z })
            .Build();
        Send(message);
    }

    public void RequestSector(int x, int y, int z) {
        if (_sectorsRequested.Contains(new Vector3(x, y, z)))
        {
            return;
        }
        _sectorsRequested.Add(new Vector3(x, y, z));

        Message message = new JsonMessageBuilder()
            .Protocol(Outbound(GridiaProtocols.Serverbound.RequestSector))
            .Set("x", x)
            .Set("y", y)
            .Set("z", z)
            .Build();
        Send(message);
    }

    public void RequestCreature(int id)
    {
        if (_creaturesRequested.Contains(id))
        {
            return;
        }
        _creaturesRequested.Add(id);

        Message message = new JsonMessageBuilder()
            .Protocol(Outbound(GridiaProtocols.Serverbound.RequestCreature))
            .Set("id", id)
            .Build();
        Send(message);
    }

    public void MoveItem(Vector3 from, Vector3 to) {
        Message message = new JsonMessageBuilder()
            .Protocol(Outbound(GridiaProtocols.Serverbound.MoveItem))
                .Set("from", new { x = from.x, y = from.y, z = from.z })
                .Set("to", new { x = to.x, y = to.y, z = to.z })
                .Build();
        Send(message);
    }

    public void Chat(String text)
    {
        Message message = new JsonMessageBuilder()
            .Protocol(Outbound(GridiaProtocols.Serverbound.Chat))
                .Set("msg", text)
                .Build();
        Send(message);
    }
}