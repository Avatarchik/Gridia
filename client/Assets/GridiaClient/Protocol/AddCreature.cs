﻿namespace Gridia.Protocol
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Serving;

    class AddCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int) data["id"];
            var name = (string) data["name"];

            var backToJson = JsonConvert.SerializeObject(data["image"]); // :(
            var image = JsonConvert.DeserializeObject<CreatureImage>(backToJson, new CreatureImageConverter());

            var x = (int) data["loc"]["x"];
            var y = (int) data["loc"]["y"];
            var z = (int) data["loc"]["z"];

            connection.GetGame().CreateCreature(id, name, image, x, y, z);
        }

        #endregion Methods
    }
}