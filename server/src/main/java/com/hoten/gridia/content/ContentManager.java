package com.hoten.gridia.content;

import com.google.gson.JsonObject;
import com.hoten.gridia.content.Item.ItemClass;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.NoSuchElementException;
import java.util.stream.Collectors;

public final class ContentManager {

    private final List<Item> _items;
    private final Map<Item, List<ItemUse>> _itemUses;
    private final List<Monster> _monsters;

    public ContentManager(List<Item> items, List<ItemUse> usesList, List<Monster> monsters) {
        _items = items;
        _itemUses = usesList.stream()
                .collect(Collectors.groupingBy(u -> getItem(u.tool), Collectors.mapping(u -> u, Collectors.toList())));
        _monsters = monsters;
        if (ItemInstance.NONE == null) {
            ItemInstance.setBlankItemInstance(new ItemInstance(_items.get(0), 0, null));
        }

        // :(
        ItemInstance decayedRemains = createItemInstance(490);
        _monsters.stream()
                .filter(monster -> monster != null)
                .forEach(monster -> monster.drops.add(decayedRemains));
    }

    public ItemInstance createItemInstance(int id, int quantity, JsonObject data) {
        if (id == 0) {
            return ItemInstance.NONE;
        }
        Item item = getItem(id);
        if (item == null) {
            System.err.println("null item: " + id);
            return ItemInstance.NONE;
        }
        return new ItemInstance(item, quantity, data);
    }

    public ItemInstance createItemInstance(int id, int quantity) {
        return createItemInstance(id, quantity, null);
    }

    public ItemInstance createItemInstance(int id) {
        return createItemInstance(id, 1, null);
    }

    public Item getItem(int id) {
        if (id == -1 || id >= _items.size()) {
            return _items.get(0);
        }
        return _items.get(id);
    }

    public Item getItemByName(String name) {
        return _items.stream()
                .filter(item -> item != null && item.name.equals(name))
                .findFirst().orElseThrow(() -> new NoSuchElementException("No such item: " + name));
    }

    public Item getItemByNameIgnoreCase(String name) {
        return _items.stream()
                .filter(item -> item != null && item.name.equalsIgnoreCase(name))
                .findFirst().orElseThrow(() -> new NoSuchElementException("No such item: " + name));
    }

    // cache?
    public Item getRandomItemOfClassByRarity(ItemClass itemClass) {
        List<Item> itemsOfClass = _items.stream()
                .filter(item -> item != null && item.itemClass == itemClass)
                .collect(Collectors.toList());
        int totalRarity = itemsOfClass.stream()
                .mapToInt(item -> item.rarity)
                .sum();
        int targetRarity = (int) (Math.random() * totalRarity);

        int raritySeen = 0;
        for (Item item : itemsOfClass) {
            raritySeen += item.rarity;
            if (raritySeen > targetRarity) {
                return item;
            }
        }

        return ItemInstance.NONE.getItem();
    }

    public Monster getMonster(int id) {
        if (_monsters.size() <= id || 0 > id) {
            return null;
        }
        return _monsters.get(id);
    }

    public List<ItemUse> getItemUses(Item tool, Item focus) {
        List<ItemUse> uses = _itemUses.get(tool);
        if (uses == null) {
            return Arrays.asList();
        }
        return uses.stream()
                .filter(u -> (u.focus == focus.id && u.focusSubType == null) || (u.focusSubType != null && u.focusSubType.equals(focus.subType)))
                .collect(Collectors.toList());
    }

    public ItemUse getItemUse(Item tool, Item focus, int index) {
        return _itemUses.get(tool).stream()
                .filter(u -> (u.focus == focus.id && u.focusSubType == null) || (u.focusSubType != null && u.focusSubType.equals(focus.subType)))
                .skip(index)
                .findFirst()
                .get();
    }

    public ItemInstance createItemInstanceByName(String itemName, int quantity) {
        return createItemInstance(getItemByNameIgnoreCase(itemName).id, quantity);
    }

    public ItemInstance createItemInstanceByName(String itemName) {
        return createItemInstanceByName(itemName, 1);
    }
}
