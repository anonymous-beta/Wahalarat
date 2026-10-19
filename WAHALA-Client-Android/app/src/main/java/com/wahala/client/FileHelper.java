package com.wahala.client;

import org.json.JSONArray;
import org.json.JSONObject;
import java.io.File;

public class FileHelper {
    public static String listDir(String path) {
        JSONArray array = new JSONArray();
        try {
            File dir = new File(path);
            if (!dir.exists() || !dir.isDirectory()) {
                return "{\"error\":\"invalid path\"}";
            }
            File[] files = dir.listFiles();
            if (files != null) {
                for (File f : files) {
                    JSONObject obj = new JSONObject();
                    obj.put("name", f.getName());
                    obj.put("dir", f.isDirectory());
                    obj.put("size", f.length());
                    obj.put("mtime", f.lastModified());
                    array.put(obj);
                }
            }
        } catch (Exception ignored) {}
        return array.toString();
    }
}