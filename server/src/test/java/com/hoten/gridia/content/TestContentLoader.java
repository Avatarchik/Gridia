package com.hoten.gridia.content;

import com.hoten.gridia.content.JsonContentLoader;
import com.hoten.gridia.content.ContentManager;
import com.hoten.gridia.content.ContentLoader;
import java.io.IOException;
import org.apache.commons.io.IOUtils;

class TestContentLoader implements ContentLoader {

    @Override
    public ContentManager load() throws IOException {
        String itemsJson = IOUtils.toString(getClass().getResourceAsStream("/test_items.json"), "UTF-8");
        String usagesJson = IOUtils.toString(getClass().getResourceAsStream("/test_usages.json"), "UTF-8");
        String monstersJson = IOUtils.toString(getClass().getResourceAsStream("/test_monsters.json"), "UTF-8");
        return new JsonContentLoader(itemsJson, usagesJson, monstersJson).load();
    }
}
