package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.ItemWrapper;
import hoten.gridia.Player;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;

public class MoveItem extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        int source = data.get("source").getAsInt();
        int dest = data.get("dest").getAsInt();
        int sourceIndex = data.get("si").getAsInt();
        int quantityToMove = data.get("quantity").getAsInt();
        int destIndex = data.get("di").getAsInt();

        if (source == dest && sourceIndex == destIndex) {
            return;
        }

        ItemWrapper sourceItemWrapped = server.getItemFrom(player, source, sourceIndex);
        ItemInstance sourceItem = sourceItemWrapped.getItemInstance();
        if (sourceItem == ItemInstance.NONE || (!player.accountDetails.isAdmin && !sourceItem.getItem().moveable)) {
            return;
        }

        if (quantityToMove == -1) { // :(
            quantityToMove = sourceItem.getQuantity();
        }
        ItemInstance itemToMove = new ItemInstance(sourceItem.getItem(), quantityToMove, sourceItem.getData());
        itemToMove.age = sourceItem.age;
        ItemWrapper destItemWrapped = server.getItemFrom(player, dest, destIndex);

        boolean moveSuccessful = destItemWrapped.addItemHere(itemToMove);
        if (!moveSuccessful) {
            return;
        }

        sourceItemWrapped.changeWrappedItem(sourceItem.remove(quantityToMove));
    }
}
