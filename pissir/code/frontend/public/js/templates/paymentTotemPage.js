"use strict";

function createExitTotemPaymentPage(){

    return `

        <div class="card my-4 p-3 shadow-lg backLog col-md-5">
            <div class="card-header text-center">
                <h4 class="myText">Puoi procedere al pagamento</h4>
            </div>
            <div class="card-body text-center">
                <!-- Costi di ricarica e sosta, centrati e distanziati -->
                <div class="mt-3"> 
                    Prezzi:
                    <span id="ricPrice" class="d-block text-start">Tariffa ricarica: </span>
                    <span id="hPrice" class="d-block text-start">Tariffa sosta: </span>
                </div>
                <h6>Dettagli servizio:</h6>
                <!-- Testo nascosto senza occupare spazio -->
                <p id="totemServiceDetails" class="d-none text-start"></p>

                <!-- Bottone "paga" -->     
                <div class="card-footer d-flex justify-content-between">   
                    <p id="priceToPay" class="myAdvertise text-start">Totale pagamento: </p>
                    <button id="paga-btn" class="btn myText myButtons">Paga</button>
                </div>
                              
            </div>
        </div>
    `
}
