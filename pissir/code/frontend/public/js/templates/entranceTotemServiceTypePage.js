"use strict";

function createEntranceTotemServiceTypePage(){

    return `

        <div class="card my-4 p-3 shadow-lg backLog col-md-5">
            <div class="card-header text-center">
                <h4 class="myText">Benvenuto!</h4>
            </div>
            <div class="card-body text-center">
                <!-- Testo nascosto senza occupare spazio -->
                <p id="totemPrenPremium" class="d-none text-start"></p>

                <!-- Testo di domanda -->
                <p class="mt-3 text-start">Desideri ricaricare la tua auto o solo effettuare una sosta?</p>

                <!-- Costi di ricarica e sosta, centrati e distanziati -->
                <div class="mt-3">
                    <span id="ricPrice" class="d-block text-start">Costo ricarica: </span>
                    <span id="hPrice" class="d-block text-start">Costo sosta: </span>
                </div>

                <!-- Bottoni sulla stessa riga, centrati -->
                <div class="d-flex justify-content-center mt-3">
                    <button id="totemRicaricaBtn" class="btn myText myButtons mx-2">Ricarica</button>
                    <button id="totemSostaBtn"class="btn myText myButtons mx-2">Sosta</button>
                </div>
                
            </div>
        </div>
    `
}
