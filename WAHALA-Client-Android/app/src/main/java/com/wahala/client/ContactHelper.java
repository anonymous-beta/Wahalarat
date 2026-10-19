package com.wahala.client;

import android.content.Context;
import android.database.Cursor;
import android.provider.ContactsContract;
import org.json.JSONArray;
import org.json.JSONObject;

public class ContactHelper {
    public static String getContacts(Context ctx) {
        JSONArray array = new JSONArray();
        try (Cursor cursor = ctx.getContentResolver().query(
                ContactsContract.CommonDataKinds.Phone.CONTENT_URI,
                null, null, null, null)) {
            if (cursor != null) {
                int nameIdx = cursor.getColumnIndex(ContactsContract.CommonDataKinds.Phone.DISPLAY_NAME);
                int numIdx  = cursor.getColumnIndex(ContactsContract.CommonDataKinds.Phone.NUMBER);
                while (cursor.moveToNext()) {
                    JSONObject c = new JSONObject();
                    c.put("name",  nameIdx >= 0 ? cursor.getString(nameIdx) : "");
                    c.put("phone", numIdx  >= 0 ? cursor.getString(numIdx)  : "");
                    array.put(c);
                }
            }
        } catch (Exception ignored) {}
        return array.toString();
    }
}