package com.wahala.client;

import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

public final class MainActivity extends AppCompatActivity {

    private TextView statusText;
    private TextView versionText;
    private TextView securityText;

    private Button statusButton;
    private Button informationButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_main);

        bindViews();
        configureInterface();
        updateStatus();
    }

    private void bindViews() {
        statusText = findViewById(R.id.statusText);
        versionText = findViewById(R.id.versionText);
        securityText = findViewById(R.id.securityText);

        statusButton = findViewById(R.id.statusButton);
        informationButton = findViewById(R.id.informationButton);
    }

    private void configureInterface() {

        statusButton.setOnClickListener(view -> {
            updateStatus();
        });

        informationButton.setOnClickListener(view -> {
            showSecurityInformation();
        });
    }

    private void updateStatus() {

        /*
         * This screen intentionally operates locally.
         *
         * It does not connect to a command server,
         * collect private device information,
         * or start a background monitoring service.
         */

        statusText.setText(
                "APPLICATION READY\n\n" +
                "The local interface is running."
        );

        versionText.setText(
                "Android API: " +
                android.os.Build.VERSION.SDK_INT
        );

        securityText.setText(
                "SECURITY MODE\n\n" +
                "Local-only mode is active.\n\n" +
                "No remote management session is connected.\n" +
                "No background monitoring service is registered.\n" +
                "No sensitive device permissions are requested."
        );
    }

    private void showSecurityInformation() {

        new androidx.appcompat.app.AlertDialog.Builder(this)
                .setTitle("About WAHALARAT")
                .setMessage(
                        "WAHALARAT Security Management\n\n" +
                        "This version provides a visible local interface.\n\n" +
                        "Remote management and sensitive device " +
                        "operations are not enabled in this build.\n\n" +
                        "Future management features should require " +
                        "explicit device-owner consent, authenticated " +
                        "connections, clear audit logs, and a visible " +
                        "way to stop or revoke access."
                )
                .setPositiveButton("Close", (dialog, which) -> {
                    dialog.dismiss();
                })
                .show();
    }

    @Override
    protected void onResume() {
        super.onResume();

        /*
         * Keep the visible status current when the user
         * returns to the application.
         */
        if (statusText != null) {
            updateStatus();
        }
    }
}