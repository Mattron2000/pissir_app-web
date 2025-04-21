"use strict";

/**
 * GUIDE DELLA MIA SALVEZZA:
 * - https://www.donskytech.com/mqtt-over-websocket/
 * - https://www.donskytech.com/how-to-enable-websockets-in-mosquitto/
 * - https://github.com/donskytech/mqtt-over-websockets/
 * - https://www.hivemq.com/blog/mqtt-client-library-mqtt-js/
 * - https://github.com/mqttjs/MQTT.js/
 */

import mqtt from "./mqtt-min.js";

// TOPICS
const TOPICS = {
  LWT: "LWT",
  SENSOR: "iot/sensors",
  MONITOR: "iot/monitor",
  MOBILE: "iot/mobiles",
  MWBOT: "iot/mwbot"
};

// MQTT CLIENT OPTIONS
const BROKER_URL = "ws://localhost";
const BROKER_PORT = 9001;
const MQTT_CLIENT_ID = "debug";
const MQTT_USERNAME = "debug";
const MQTT_PASSWORD = "debug";
const LWT_BODY = {
  id: MQTT_CLIENT_ID,
  message: `[DEBUG] id='${MQTT_CLIENT_ID}' : I'm gone. Bye.`
}

const client = mqtt.connect(BROKER_URL, {
  clientId: MQTT_CLIENT_ID,
  username: MQTT_USERNAME,
  password: MQTT_PASSWORD,
  port: BROKER_PORT,
  will: {
    topic: TOPICS.LWT,
    payload: JSON.stringify(LWT_BODY),
    qos: 0,
    retain: false
  }
});

const SENSOR_COUNT = 10;

let sensorHashMap = new Map();
let mobileHashMap = new Map();

client.on('connect', function () {
    console.log('Connected');
    // client.subscribe(['iot/sensors/+/status', 'iot/monitor/count', 'iot/mobiles/+/data'], function (err) {
    client.subscribe('#', function (err) {
        if (err) {
            console.log('Subscription error:', err);
        } else {
            console.log('Subscribed to topics');
        }
    });
});

client.on('error', function (err) {
  console.log('Client error:', err);
});

client.on('close', function () {
  console.log('Connection closed');
});

function getDatetimeNow() {
  const now = new Date();

  let year = now.getFullYear();

  let months = now.getMonth() + 1;
  months < 10 ? months = months = "0" + months: '';

  let days = now.getDate();
  days < 10 ? days = days = "0" + days: '';

  let hours = now.getHours();
  hours < 10 ? hours = hours = "0" + hours: '';

  let minutes = now.getMinutes();
  minutes < 10 ? minutes = minutes = "0" + minutes: '';

  let seconds = now.getSeconds();
  seconds < 10 ? seconds = seconds = "0" + seconds: '';

  return year + "-" + months + "-" + days + " " + hours + ":" + minutes + ":" + seconds;
}

// messageArrived(String topic, MqttMessage message)
client.on("message", (topic, message) => {
  // message is Buffer
  message = message.toString();
  console.log("Received message: " + message + " from topic: " + topic);
  let bodyMqttMessages = document.getElementById("accordion-body-mqtt-messages")

  bodyMqttMessages.innerHTML = "<p><b style='color:gray'>" + getDatetimeNow() + "</b> <b>" + topic + "</b> " + message + "</p>" + bodyMqttMessages.innerHTML;

  if (topic.match("^" + TOPICS.SENSOR + "/sensor_\\d+/status$")) {
    try {
      const sensorData = JSON.parse(message);
      sensorHashMap.set(sensorData.slot_id, sensorData.status);
    } catch (error) {
      catchErrorModal(error, "sensorData = JSON.parse(message);");
    }
    updateSensorsView();
    return;
  }

  if (topic.match("^" + TOPICS.MONITOR + "/count$")) {
    let monitorData = null;

    try {
      monitorData = JSON.parse(message);
    } catch (error) {
      catchErrorModal(error, "monitorData = JSON.parse(message);");
    }

    if (monitorData !== null)
      updateMonitorView(monitorData.count);

    return;
  }

  if (topic.match("^" + TOPICS.MOBILE + "/(\\d{3}[- .]?){2}\\d{4}/data$")) {
    try {
      const mobileData = JSON.parse(message);
      mobileHashMap.set(mobileData.id, mobileData.message);
    } catch (error) {
      catchErrorModal(error, "mobileData = JSON.parse(message);");
    }
    updateMobilesView();
    return;
  }

  if (topic.match("^" + TOPICS.MWBOT + "/debug$")) {
    let mwbotData = null;

    try {
      mwbotData = JSON.parse(message);
    } catch (error) {
      catchErrorModal(error, "mwbotData = JSON.parse(message);");
    }

    if (mwbotData !== null)
      updateMWbotView(mwbotData);

    return;
  }

  if (topic.match("^" + TOPICS.MOBILE + "/\\d+/new$")) {
    let mobileData = null;

    try {
      mobileData = JSON.parse(message);
    } catch (error) {
      catchErrorModal(error, "mobileData = JSON.parse(message);");
    }

    if (mobileData === null) {
      console.error("mobileData === null");
      return;
    }

    runNewMobileJar(mobileData.phoneNumber);
    return;
  }

  console.log("Unknown topic: " + topic)
});

// get all sensors output
client.subscribe(TOPICS.SENSOR + "/+/status", { qos: 0 });
// get monitor output
client.subscribe(TOPICS.MONITOR + "/count", { qos: 0 });
// get mobiles output
client.subscribe(TOPICS.MOBILE + "/+/data", { qos: 0 });
// get mwbot output
client.subscribe(TOPICS.MWBOT + "/debug", { qos: 0 });
// get new mobile request from backend
client.subscribe(TOPICS.MOBILE + "/+/new", { qos: 0 });

// ----------------------------------------------------------------------------
// MOBILES
// ----------------------------------------------------------------------------

function runNewMobileJar(phoneNumber) {
  fetch("http://localhost:8080/setupNewMobile", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      phoneNumber: phoneNumber
    })
  })
  .catch(error => catchErrorModal(error, "Failed to create mobile"));
}

function setupNewMobile() {
  // pattern="[0-9]{10}"
  const phoneNumber = document.getElementById("mobileInputPhoneNumber").value;

  if(phoneNumber.match("^[0-9]{10}$") === null) {
    catchErrorModal("Setup New Mobile", "Invalid phoneNumber");
    return;
  }

  runNewMobileJar(phoneNumber);
}

document.getElementById("mobileFormSubmit").addEventListener("click", () => setupNewMobile());

// ----------------------------------------------------------------------------
// SENSORS
// ----------------------------------------------------------------------------

function updateSensorsView() {
  let sensorCards = [];
  for (const [id, status] of sensorHashMap)
    sensorCards.push(createSensorCard(id, status));

  sensorCards.sort((a, b) => {
    return Number(a - b);
  });

  document.getElementById("accordion-body-sensor").innerHTML = sensorCards.join('');

  for (const id of sensorHashMap.keys())
    document.getElementById(id).addEventListener("click",() => switchSensorStatus(id));
}

function createSensorCard(id, status) {
  return `
  <div class="card-sensor card bg-${status === "FREE" ? "success" : "danger"} bg-opacity-25 m-1" type="button" id="${id}">
    <div class="card-body text-center">
      <h5 class="card-title">sensor_${id}</h5>
      <p class="card-text">${status}</p>
    </div>
  </div>
  `
}

function switchSensorStatus(id) {
  client.publish(`iot/sensors/sensor_${id}/switch`, "", {
    qos: 0,
    retain: false,
  });
}

function updateSensors() {
  if (document.getElementById("accordion-item-sensor-button").classList.contains("collapsed"))
    return;

  sensorHashMap.clear();
  document.getElementById("accordion-body-sensor").innerHTML = "";

  let i = 1;

  const interval = setInterval(() => {
    client.publish(`iot/sensors/sensor_${i}/check`, "", {
      qos: 0,
      retain: false,
    });
    i++;

    if (i > SENSOR_COUNT)
      clearInterval(interval);
  }, 1000);
}

document.getElementById("accordion-item-sensor-button").addEventListener("click", updateSensors);

// ----------------------------------------------------------------------------
// MONITOR
// ----------------------------------------------------------------------------

function updateMonitorView(newCount) {
  const count = document.getElementById("accordion-body-monitor").innerHTML = `Free slots: ${Number(newCount)}`;
}

function updateMonitor() {
  if (document.getElementById("accordion-item-monitor-button").classList.contains("collapsed"))
    return;

  client.publish(`iot/monitor/check`, "", {
    qos: 0,
    retain: false,
  });
}

document.getElementById("accordion-item-monitor-button").addEventListener("click", updateMonitor);

// ----------------------------------------------------------------------------
// MWBOT
// ----------------------------------------------------------------------------

function updateMWbotView(mwbotData) {
  document.getElementById("mwbot-status").innerHTML = mwbotData.status;
  document.getElementById("mwbot-position").innerHTML = mwbotData.position;
  document.getElementById("mwbot-model").innerHTML = mwbotData.model === "" ? "NULL" : mwbotData.model;
  document.getElementById("mwbot-percentage").innerHTML = mwbotData.percentage === 0 ? "NULL" : mwbotData.percentage;
}

function updateMWbot() {
  if (document.getElementById("accordion-item-mwbot-button").classList.contains("collapsed"))
    return;

  client.publish(`iot/mwbot/check`, "", {
    qos: 0,
    retain: false,
  });
}

document.getElementById("accordion-item-mwbot-button").addEventListener("click", updateMWbot);

// ----------------------------------------------------------------------------
// MOBILES
// ----------------------------------------------------------------------------

function updateMobilesView() {
  let mobileCards = [];
  for (const [id, message] of mobileHashMap)
    mobileCards.push(createMobileCard(id, message));

  mobileCards.sort((mobile_a, mobile_b) => {
    const regex = /\d{10}|(\d{3}[- .]?){2}\d{4}/;
    // '1231231231', '123-123-1231' or '123 123 1231'
    const phone_number_a = mobile_a.match(regex)[0];
    const phone_number_b = mobile_b.match(regex)[0];

    return Number(phone_number_a - phone_number_b);
  });

  document.getElementById("accordion-body-mobile").innerHTML = mobileCards.join('');

  for (const id of mobileHashMap.keys())
    document.getElementById(`${id}`).addEventListener("click",() => closeMobile(id));
}

function createMobileCard(id, message) {
  return `
  <div class="card-mobile card bg-${message === "" ? "secondary" : "success"} bg-opacity-25 m-1" type="button">
    <div class="card-body text-center">
      <h5 class="card-title">${id}</h5>
      <p class="card-text">${message}</p>
      <button type="button" class="btn btn-secondary" id="${id}">Close</button>
    </div>
  </div>
  `
}

function closeMobile(id) {
  client.publish("iot/mobiles/" + id + "/close", " ", {
    qos: 0,
    retain: false,
  });

  mobileHashMap.delete(id);

  updateMobilesView();
}

// ----------------------------------------------------------------------------
// ERROR MODAL
// ----------------------------------------------------------------------------

let errorModalLaunched = false;

function catchErrorModal(error, modalTitle) {
  if (isAlreadyLaunched())
    return;

  const errorModalTitle = document.querySelector('#error-modal .modal-title');
  const errorModalBody = document.querySelector('#error-modal .modal-body');
  errorModalTitle.textContent = error.name + ": " + modalTitle;
  errorModalBody.textContent = error.message;

  new bootstrap.Modal('#error-modal').show();
}

function isAlreadyLaunched() {
  if (errorModalLaunched === false) {
    errorModalLaunched = true;
    return false
  }

  return errorModalLaunched;
}

function closeErrorModal() {
  errorModalLaunched = false;
}

document.getElementById("error-modal-btn-close").addEventListener("click", closeErrorModal);
