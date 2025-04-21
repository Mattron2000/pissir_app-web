"use strict";

function createAdminPage(){

    return `
    <div class="container-fluid adminContainer">
    <div class="container">
        <div class="row">
            <div class="col-12 bg-light p-3 text-center blackBackground pt-3 pb-2 mb-3 border-bottom">
                <h5 class="myText blackBackground">Benvenuto nell'area amministratore. Seleziona un'opzione dal menu per continuare.</h5>
            </div>
        </div>
 
        <div class="row">
            <!-- Menu laterale -->
            <nav class="col-md-4 col-lg-3 d-md-block md sidebar navMenu">
                <div class="list-group">
                    <h5 class="list-group-item myText blackBackground left noMargin">Menu</h5>
                    <button class="list-group-item list-group-item-action left myAdmMenu" id="costo-orario-btn" data-toggle="modal" data-target="#updateHoursModal">Aggiorna prezzo orario</button>
                    <button class="list-group-item list-group-item-action left myAdmMenu" id="costo-kw-btn" data-toggle="modal" data-target="#updateKwModal">Aggiorna prezzo kw</button>
                    <button class="list-group-item list-group-item-action left myAdmMenu" id="monitoraggio-parcheggi-btn">Monitoraggio parcheggi</button>
                    <button class="list-group-item list-group-item-action left myAdmMenu" id="ricerca-pagamenti-btn" data-toggle="modal" data-target="#paymentHistoryModal">Cronologia pagamenti</button>
                </div>
            </nav>

            <!-- Sezione dei risultati -->
            <div class="col-md-8 ml-sm-auto col-md-4 px-md-3" id="result-section">
                <div id="dynamic-content">
                    <h5 class="myText" style="margin-top: 5px; text-align: center;">Qui sotto verranno mostrati i risultati delle tue ricerche</h5>
                    
                    <!-- Tabella risultati -->                
                    <table class="table mt-4 table-responsive-lg" id="admTable">
                    
                    </table>
                         
                </div>
            </div>
        </div>
    </div>
</div>

    `
}