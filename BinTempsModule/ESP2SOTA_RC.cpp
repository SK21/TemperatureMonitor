#include "ESP2SOTA_RC.h"
#include "index_html.h"

//Class constructor
ESP2SOTAClass::ESP2SOTAClass() {}

#if defined(ESP8266)
void ESP2SOTAClass::begin(ESP8266WebServer *server) {
#elif defined(ESP32)
void ESP2SOTAClass::begin(WebServer *server) {
#endif
    _server = server;

    // Serve OTA upload page
    _server->on("/update", HTTP_GET, [&]() {
        _server->sendHeader("Connection", "close");
        _server->send(200, "text/html", indexHtml);
    });

    // Handle firmware upload
    _server->on("/update", HTTP_POST, [&]() {
        _server->sendHeader("Connection", "close");
        _server->send(200, "text/plain", (Update.hasError()) ? "FAIL" : "OK");
        ESP.restart();
    }, [&]() {
        HTTPUpload& upload = _server->upload();

        if (upload.status == UPLOAD_FILE_START) {
            Serial.printf("Update: %s\n", upload.filename.c_str());

#if defined(ESP8266)
            // ESP8266 requires explicit max sketch space
            size_t maxSketchSpace =
                (ESP.getFreeSketchSpace() - 0x1000) & 0xFFFFF000;

            if (!Update.begin(maxSketchSpace)) {
                Update.printError(Serial);
            }
#else
            // ESP32 supports UPDATE_SIZE_UNKNOWN
            if (!Update.begin(UPDATE_SIZE_UNKNOWN)) {
                Update.printError(Serial);
            }
#endif
        }

        else if (upload.status == UPLOAD_FILE_WRITE) {
            if (Update.write(upload.buf, upload.currentSize) != upload.currentSize) {
                Update.printError(Serial);
            }
        }

        else if (upload.status == UPLOAD_FILE_END) {
            if (Update.end(true)) {
                Serial.printf("Update Success: %u bytes\nRebooting...\n", upload.totalSize);
            } else {
                Update.printError(Serial);
            }
        }
    });
}

ESP2SOTAClass ESP2SOTA;
