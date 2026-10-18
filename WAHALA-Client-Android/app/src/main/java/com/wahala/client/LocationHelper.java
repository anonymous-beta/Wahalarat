package com.wahala.client;

import android.content.Context;
import android.location.Location;
import android.location.LocationManager;

public class LocationHelper {
    public static void getLocation(Context ctx, Network net) {
        LocationManager lm = (LocationManager) ctx.getSystemService(Context.LOCATION_SERVICE);
        try {
            Location loc = lm.getLastKnownLocation(LocationManager.GPS_PROVIDER);
            if (loc == null) loc = lm.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
            
            if (loc != null) {
                String json = String.format("{\"lat\":%f,\"lon\":%f}", loc.getLatitude(), loc.getLongitude());
                net.send(json);
            }
        } catch (SecurityException ignored) {}
    }
}
