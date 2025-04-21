"use strict";   

class AppEntranceTotem{

    
    constructor(entranceTotemappContainer){


        this.appContainer= entranceTotemappContainer;
        this.utenteLoggato= null;   //contiene id utente, username e grado

        this.homeListenersDefined= false;

        /* routes Page.js */

        /* login page */
        page('/entranceLogin', () =>{
            this.appContainer.innerHTML= '';    //appContainer è Element
            this.showEntranceLoginPage();
        });

        /* sosta/ricarica page */
        page('/entranceServiceType', () =>{
            this.appContainer.innerHTML= '';    //appContainer è Element
            this.showEntranceServiceTypePage();
        });


        page('*', () => {
            page('/entranceLogin');
        });

        /* inizializzazione, così fa binding di tutte le funzioni sopra*/
        page();
    }



    showEntranceLoginPage(){
        
        this.appContainer.innerHTML= createEntranceTotemLoginPage();  //definita nel template
        this.addEntranceTotemLoginListener();
        if (this.homeListenersDefined == true) 
            return; 

        this.homeListenersDefined = false; 
    
    }

    /* fa partire login */
    addEntranceTotemLoginListener(){

        //faccio partire login quando utente invia form
        document.getElementById('loginBtnEntranceTotem').addEventListener('click', async(event) =>{
            event.preventDefault();

            let username= document.getElementById('login-form-email').value;
            let password= document.getElementById('login-form-password').value;

            let response= await Api.loginUser(username, password);

            if(response['error']){  //se ho inserito mail o password sbagliate

                let error= response['error'];
                let username= document.getElementById('login-form-email').value;
                let password= document.getElementById('login-form-password').value;

                //se è sbagliato username
                if(error.campo === 'username'){
                    username= document.getElementById('login-form-email');
                    username.classList.add('is-invalid');

                    password= document.getElementById('login-form-password');
                    if (password.classList.contains('is-invalid'))
                        password.classList.remove('is-invalid');
                    document.getElementById('usernameError').innerText= error.message;
                }

                //se è sbagliata password
                if(error.campo === 'password'){
                    username= document.getElementById('login-form-email');
                    if (username.classList.contains('is-invalid'))
                        username.classList.remove('is-invalid');
                    password= document.getElementById('login-form-password');
                    password.classList.add('is-invalid');
                    document.getElementById('passwordError').innerText= error.message;
                }
                document.getElementById("errorModalBody").innerText= response.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }

            //se login è avvenuto correttamente, carico pagina sosta/prenota
            this.utenteLoggato= response;   //profileID, username e grado
            page('/entranceServiceType');

        });
    }

    checkPrenotazione(myPrenotation) {
        // Ottengo la data e l'ora correnti
        let now = new Date();

        // Calcolo la differenza in millisecondi*/
        let arrivo = new Date(myPrenotation.datetime_start);
        let diffInMilliseconds = Math.abs(now.getTime() - arrivo.getTime());
        // Converto la differenza in minuti
        let diffInMinutes = diffInMilliseconds / (1000 * 60);

        // Controllo se la differenza è entro 15 minuti
        if(diffInMinutes <= 15)
            return myPrenotation;
        return null;
    }

    /* dopo login, si visualizza il tipo di sosta da scegliere*/
    async showEntranceServiceTypePage(){

        this.appContainer.innerHTML= createEntranceTotemServiceTypePage();  //definita nel template

        //ottengo i prezzi 
        let prices= await Api.getPrices();
        if(prices.error){  

            document.getElementById("errorModalBody").innerText= prices.error;
            $('#errorModal').modal('show');
            return; //si blocca la richiesta.
        }
       
        let kwPrice= prices.costo_kw + "€/kw";
        let hoursPrice= prices.costo_sosta + "€/h";

        document.getElementById("ricPrice").textContent += kwPrice;
        document.getElementById("hPrice").textContent += hoursPrice;

        
        //riempi box con prenotazioni, se ci sono
        if(this.utenteLoggato.type== "PREMIUM"){ 
            //cerca in db dove "pagato=0" (prenotazione in sospeso)
            let myPrenotations= await Api.getMyPrenotations(this.utenteLoggato.email);
            if(myPrenotations.error){  

                document.getElementById("errorModalBody").innerText= myPrenotations.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }
            if(myPrenotations.length != 0) {
                
                //guardo se la prenotazione è per ADESSO.
                //N.B. utente può arrivare fino a 15 minuti prima o 15 minuti dopo
                let isPren = null;
                for (let prenotation of myPrenotations) {
                    isPren = this.checkPrenotazione(prenotation); // Torna prenotazione se è per orario corrente, null altrimenti
                    if (isPren) 
                        break;
                }
                
                                    
                if(isPren){
                    let orarioInizio= isPren.datetime_start.replace("T", " ");
                    let orarioFine= isPren.datetime_end.replace("T", " ");
                    document.getElementById("totemPrenPremium").innerHTML = 
                        `Hai una prenotazione per adesso!<br>
                        Data di arrivo: ${orarioInizio}<br>
                        Data di fine: ${orarioFine}<br>
                        ID parcheggio assegnato: ${isPren.slot_id}`;
                        
                    // Rendo visibile il paragrafo
                    document.getElementById("totemPrenPremium").classList.remove("d-none");     
                    
                    //se premium con prenotazione schiaccia sosta
                    $('#totemSostaBtn').one('click', async () =>{

                        let sostaPremiumModal = new bootstrap.Modal(document.getElementById('sostaModalOk'));
                        document.getElementById("sostaOkLabel").textContent = `Puoi parcheggiare il tuo veicolo al posto numero ${isPren.slot_id}`;
                        sostaPremiumModal.show();

                        /* tolgo riga da tabella reservation e aggiungo riga alle request */
                        let deleteData= {};
                        deleteData.email= this.utenteLoggato.email;
                        deleteData.inizio= isPren.datetime_start;
                        let deleteResponse= await Api.deletePrenotation(deleteData);  
                        if(deleteResponse.error){  

                            document.getElementById("errorModalBody").innerText= deleteResponse.error;
                            $('#errorModal').modal('show');
                            return; //si blocca la richiesta.
                        }
                        let modParkingData={};
                        modParkingData.email= this.utenteLoggato.email;
                        modParkingData.slot_id= isPren.slot_id; //id parcheggio
                        modParkingData.datetime_start= isPren.datetime_start;                       
                        modParkingData.datetime_end=isPren.datetime_end;
                        modParkingData.percentage= null;
                        modParkingData.reservation= false;
                        
                      
                        let insertResponse= await Api.insertDetails(modParkingData);
                        if(insertResponse.error){  

                            document.getElementById("errorModalBody").innerText= insertResponse.error;
                            $('#errorModal').modal('show');
                            return; //si blocca la richiesta.
                        }

                        //logout e torno su pagina principale
                        setTimeout(() => {
                            this.utenteLoggato= null;
                            sostaPremiumModal.hide();
                            page('/entranceLogin');
                        }, 8000);
                    });

                    //se premium prenotato schiaccia ricarica
                    let self = this;
                    $('#totemRicaricaBtn').one('click', async () =>{
                        let carQueue = await Api.getCarQueue();
                        self.aggiornaInfoCoda(carQueue);
                        let ricaricaModal = new bootstrap.Modal(document.getElementById('modalRicarica'));
                        
                        if(!(document.getElementById("tempoFineGroup").classList.contains("d-none")))
                            document.getElementById("tempoFineGroup").classList.add("d-none");
                        document.getElementById("modalLabelRicarica").innerHTML = "Inserisci la percentuale di batteria da ricaricare, e il numero di telefono su cui desideri ricevere una notifica a ricarica terminata.";
                    
                        ricaricaModal.show();                       
                        self.addRicaricaListener(ricaricaModal, isPren.datetime_start, isPren.datetime_end, isPren.slot_id);

                    });

                    return;
                }
            }           
        }

        //se utente base o premium senza prenotazione schiacciano "sosta"

        let self=this;
        //se si schiaccia sosta
        $('#totemSostaBtn').one('click', () =>{

            let sostaNormaleModal = new bootstrap.Modal(document.getElementById('modalSostaNormale'));
            sostaNormaleModal.show();

            $('#modal-sosta-Normale-btn').one('click', async() =>{
                let tPermanenzaInputNormale = document.getElementById('tempoPermanenzaNormale');
                let errorMessageSostaNormale = document.getElementById('errorMessageSostaNormale');
                let errorMessageSostaNormaleNotAvailable = document.getElementById('errorMessageSostaNormaleNotAvailable');
            
                let tPermValueNormale= tPermanenzaInputNormale.value;
                tPermanenzaInputNormale.classList.remove('is-invalid');
                errorMessageSostaNormale.style.display = 'none'; 
                errorMessageSostaNormaleNotAvailable.style.display= 'none';

                // Verifica che sia inserito il tempo di permanenza
                if (!tPermValueNormale) {      
                    errorMessageSostaNormale.style.display = 'block'; 
                    tPermanenzaInputNormale.classList.add('is-invalid'); 
                    return;          
                } 

                let now1= new Date();

                // Estrai l'ora nel formato hh:mm:ss
                let tArrivo = now1.getHours().toString().padStart(2, '0') + ':' + 
                now1.getMinutes().toString().padStart(2, '0') + ':' + 
                now1.getSeconds().toString().padStart(2, '0');

                // Estrai la data nel formato yyyy-mm-gg
                let dataSosta = now1.getFullYear() + '-' + 
                (now1.getMonth() + 1).toString().padStart(2, '0') + '-' + 
                now1.getDate().toString().padStart(2, '0');

                let tempoSostaInt = parseInt(tPermValueNormale, 10); // Base 10

                //creo orario fine
                let arrivingTimeMillisec= now1.getTime();
                let tempoRicMillisec= (tempoSostaInt * 60) * 1000;
                let finalTimeDate = new Date(arrivingTimeMillisec + tempoRicMillisec);

                /* mi servono stringa con solo data e stringa con solo ora */

                //solo data
                let annoFinePrenotazione = finalTimeDate.getFullYear();
                let meseFinePrenotazione = String(finalTimeDate.getMonth() + 1).padStart(2, '0'); // Mesi da 0 a 11, quindi +1
                let giornoFinePrenotazione = String(finalTimeDate.getDate()).padStart(2, '0');           
                // Formatta la data in "yyyy-mm-dd"
                let formattedDate = `${annoFinePrenotazione}-${meseFinePrenotazione}-${giornoFinePrenotazione}`;

                //solo ora
                // Estraggo i componenti dell'ora
                let hours = String(finalTimeDate.getHours()).padStart(2, '0');
                let minutes = String(finalTimeDate.getMinutes()).padStart(2, '0');
                let seconds = String(finalTimeDate.getSeconds()).padStart(2, '0');

                // Formatta l'ora in "hh:mm:ss"
                let formattedTime = `${hours}:${minutes}:${seconds}`;


                let dataInizioCompleta = `${dataSosta}T${tArrivo}`;
                let dataFineCompleta = `${formattedDate}T${formattedTime}`;
                let datiSostaNormale= {};
                datiSostaNormale.arrivingTime=dataInizioCompleta;
                datiSostaNormale.endTime=dataFineCompleta;
                

                let myParking= await Api.getMyCarParkingSosta(datiSostaNormale); 
                if(myParking.error == "No free slots"){ 
                    errorMessageSostaNormaleNotAvailable.style.display = 'block';
                    document.getElementById("errorModalBody").innerText= myParking.error;
                    $('#errorModal').modal('show');
                    //logout e torno su pagina principale
                    setTimeout(() => {
                        this.utenteLoggato= null;
                        errorMessageSostaNormaleNotAvailable.style.display = 'none';
                        $('#errorModal').modal('hide');
                        sostaNormaleModal.hide();
                        page('/entranceLogin');
                    }, 8000);

                }else if(myParking.error){
                    document.getElementById("errorModalBody").innerText= myParking.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }else{
                    sostaNormaleModal.hide();
                    /* aggiungo dettagli servizio al db */
                    let newParkingData= {};
                    newParkingData.email= self.utenteLoggato.email;
                    newParkingData.slot_id= myParking;
                    newParkingData.datetime_start= dataInizioCompleta;
                    newParkingData.datetime_end = dataFineCompleta;
                    newParkingData.percentage= null;
                    newParkingData.reservation= false;

                    let regResponse= await Api.insertDetails(newParkingData);
                    if(regResponse.error){  

                        document.getElementById("errorModalBody").innerText= regResponse.error;
                        $('#errorModal').modal('show');
                        return; //si blocca la richiesta.
                    }

                    let sostaModalOk = new bootstrap.Modal(document.getElementById('sostaModalOk'));
                    document.getElementById("sostaOkLabel").textContent = `Puoi parcheggiare il tuo veicolo al posto numero ${myParking}`;
                    sostaModalOk.show();

                    //logout e torno su pagina principale
                    setTimeout(() => {
                        this.utenteLoggato= null;
                        sostaModalOk.hide();
                        page('/entranceLogin');
                    }, 8000);
                    
                }
            });

        });

        //se si schiaccia ricarica
        $('#totemRicaricaBtn').one('click', async () =>{

            let ricaricaNormaleModal = new bootstrap.Modal(document.getElementById('modalRicarica'));

            if(document.getElementById("tempoFineGroup").classList.contains("d-none"))
                document.getElementById("tempoFineGroup").classList.remove("d-none");
            document.getElementById("modalLabelRicarica").innerHTML = "Inserisci la percentuale di batteria da ricaricare, l'ora di fine e il numero di telefono su cui desideri ricevere una notifica a ricarica terminata.";
            
            ricaricaNormaleModal.show();

            /* get coda auto */
            let carQueue = await Api.getCarQueue();
            self.aggiornaInfoCoda(carQueue);
            self.addRicaricaListener(ricaricaNormaleModal, null, null, null);

        });
        

    }

    /* popola modal ricarica con info su coda e attesa */
    aggiornaInfoCoda(carQueue) {
        let infoCoda = document.getElementById('infoCoda');    
        infoCoda.innerHTML = `Numero di auto in coda prima di me: ${carQueue}.`;
        
        // Mostro il messaggio
        infoCoda.style.display = 'block';
    }

    /*listener ricarica*/
    addRicaricaListener(ricaricaModal, oraInizioPrenotazione, oraFinePrenotazione, myParkingIdParcheggioPrenotazione){

        let self=this;
        $('#parcheggia-e-carica-btn').one('click',  async () => {
            let validationRes= self.ricaricaFormValidation();

            if(validationRes == null)
                return;
            
            let ricaricaForm= document.getElementById("ricarica-form");
            let percentualeRicInput = ricaricaForm.elements['percentuale-ricarica'];
            let percentualeRic = percentualeRicInput.value;

            let tempoFineInput;
            let tempoFine;
            if(!myParkingIdParcheggioPrenotazione){   //c'è tempoFine
                tempoFineInput= ricaricaForm.elements['tempoFine'];
                tempoFine= tempoFineInput.value;
            }


            let timeRicModal = new bootstrap.Modal(document.getElementById('timeRicModal'));
            document.getElementById("timeRicLabel").textContent = `La tua auto si sarà caricata del ${percentualeRic}%.`;
            timeRicModal.show();

            $('#confermaRicBtn').one('click', async () =>{
                //mi servono data inizio e fine complete
                //se prenotato ho già ora inizio e ora fine
                //se non prenotato calcolo ora inizio e poi ora fine

                /* calcolo ora inizio e fine da passare a Api.getMyCarParkingSosta */
                let dataInizioCompleta;
                let dataFineCompleta;
                let datiRicNormale= {};
                if(!myParkingIdParcheggioPrenotazione){   //c'è tempoFine
                    let now1= new Date();
                    // Estraggo l'ora nel formato hh:mm:ss
                    let tArrivo = now1.getHours().toString().padStart(2, '0') + ':' + 
                    now1.getMinutes().toString().padStart(2, '0') + ':' + 
                    now1.getSeconds().toString().padStart(2, '0');

                    // Estraggo la data nel formato yyyy-mm-gg
                    let dataRic = now1.getFullYear() + '-' + 
                        (now1.getMonth() + 1).toString().padStart(2, '0') + '-' + 
                        now1.getDate().toString().padStart(2, '0');

                    //creo orario di fine
                    let oraFine= tempoFine + ":00";
                    //se orario fine è precedente a orario corrente, la data di fine è al giorno dopo
                    let dataFineRic= dataRic;
                    let tempDateObj= new Date(`${dataRic}T${oraFine}`);
                    if(tempDateObj.getTime() < now1.getTime()){
                        tempDateObj.setDate(tempDateObj.getDate() + 1); // Aggiunge 1 giorno
                        dataFineRic= tempDateObj.getFullYear() + '-' + 
                            (tempDateObj.getMonth() + 1).toString().padStart(2, '0') + '-' + 
                            tempDateObj.getDate().toString().padStart(2, '0');
                    }

                    dataInizioCompleta = `${dataRic}T${tArrivo}`;
                    dataFineCompleta = `${dataFineRic}T${oraFine}`;
                    datiRicNormale.arrivingTime=dataInizioCompleta;
                    datiRicNormale.endTime=dataFineCompleta;
                }

                let myParking= myParkingIdParcheggioPrenotazione;

                //guardo se c'è almeno un parcheggio libero, quindi guardo anche tutte le soste, e ottengo IDParcheggio.
                if(!myParkingIdParcheggioPrenotazione)
                    myParking= await Api.getMyCarParkingSosta(datiRicNormale); 
                
                if(myParking== null){ 
                    errorMessageRicaricaNormaleNotAvailable.style.display = 'block';
                }
                else{ 

                    timeRicModal.hide();
                    ricaricaModal.hide();
                                
                    /* aggiungo dettagli servizio al db */
                    if(!myParkingIdParcheggioPrenotazione){   //se non era prenotato, faccio insert
                        let newParkingData= {};
                        newParkingData.email= self.utenteLoggato.email;
                        newParkingData.slot_id= myParking;
                        newParkingData.datetime_start= dataInizioCompleta;
                        newParkingData.datetime_end = dataFineCompleta;
                        newParkingData.percentage= percentualeRic;
                        newParkingData.reservation= false;
                        newParkingData.phone_number= validationRes;
                        let regResponse= await Api.insertDetails(newParkingData);
                        if(regResponse.error){  

                            document.getElementById("errorModalBody").innerText= regResponse.error;
                            $('#errorModal').modal('show');
                            return; //si blocca la richiesta.
                        }
                    }else{  //se era prenotato, devo fare una put alla riga già esistente

                        let deleteData= {};
                        deleteData.email= this.utenteLoggato.email;
                        deleteData.inizio= oraInizioPrenotazione;
                        let deleteResponse= await Api.deletePrenotation(deleteData);  
                        if(deleteResponse.error){  

                            document.getElementById("errorModalBody").innerText= deleteResponse.error;
                            $('#errorModal').modal('show');
                            return; //si blocca la richiesta.
                        }
                        let modParkingData={};
                        modParkingData.email= this.utenteLoggato.email;
                        modParkingData.slot_id= myParkingIdParcheggioPrenotazione; //id parcheggio
                        modParkingData.datetime_start= oraInizioPrenotazione;                       
                        modParkingData.datetime_end=oraFinePrenotazione;
                        modParkingData.percentage= percentualeRic;
                        modParkingData.reservation= false;
                        modParkingData.phone_number= validationRes;
                      
                        let insertResponse= await Api.insertDetails(modParkingData);
                        if(insertResponse.error){  

                            document.getElementById("errorModalBody").innerText= insertResponse.error;
                            $('#errorModal').modal('show');
                            return; //si blocca la richiesta.
                        }

                    }
                    //chiudo modal
                    ricaricaModal.hide();
                    
                    //è uguale anche se sosta
                    let sostaModalOk = new bootstrap.Modal(document.getElementById('sostaModalOk'));
                    if(!myParkingIdParcheggioPrenotazione)
                        document.getElementById("sostaOkLabel").textContent = `Puoi parcheggiare il tuo veicolo al posto numero ${myParking}`;
                    else
                        document.getElementById("sostaOkLabel").textContent = `Puoi parcheggiare il tuo veicolo al posto numero ${myParkingIdParcheggioPrenotazione}`;
                    sostaModalOk.show();

                    //logout e torno su pagina principale
                    setTimeout(() => {
                        this.utenteLoggato= null;
                        sostaModalOk.hide();
                        page('/entranceLogin');
                    }, 8000);
                }
                
            });           
        });
    }

    ricaricaFormValidation(){

        let ricaricaForm= document.getElementById("ricarica-form");
        let percentualeRicInput = ricaricaForm.elements['percentuale-ricarica'];
        let percentualeRic = percentualeRicInput.value;
        let phoneNumber= ricaricaForm.elements['phoneNumber'];
        

        var errorMessageRicarica = document.getElementById('errorMessageRicarica');
        var errorMessageRicarica2 = document.getElementById('errorMessageRicarica2');
        var errorMessageRicarica4 = document.getElementById('errorMessageRicarica4');
        var errorMessageRicaricaNormaleNotAvailable = document.getElementById('errorMessageRicaricaNormaleNotAvailable');
        var errorMessagePhoneNumber = document.getElementById('errorMessagePhoneNumber');
        var errorMessagePhoneNumber2 = document.getElementById('errorMessagePhoneNumber2')
        

        // Rimuovo le classi di errore precedenti
        percentualeRicInput.classList.remove('is-invalid');
        phoneNumber.classList.remove('is-invalid');
        errorMessageRicarica.style.display = 'none';
        errorMessageRicarica2.style.display = 'none';
        errorMessageRicarica4.style.display = 'none'; 
        errorMessageRicaricaNormaleNotAvailable.style.display = 'none';   
        errorMessagePhoneNumber.style.display= 'none';
        errorMessagePhoneNumber2.style.display= 'none';


        if(!percentualeRicInput){
            errorMessageRicarica.style.display = 'block';
            percentualeRicInput.classList.add('is-invalid');

            return null;
        }
        
        /* verifico che i dati inseriti siano nel formato valido  */
        if(percentualeRic > 100){   //altri formati non validi non possono essere messi dall'html
            errorMessageRicarica2.style.display = 'block';
            percentualeRicInput.classList.add('is-invalid');
            return null;
        }
        

        /* SE C'è TEMPO FINE */ //no prenotazione
        if(document.getElementById("totemPrenPremium").classList.contains("d-none")){  
            let tempoFineInput= ricaricaForm.elements['tempoFine'];
            let tempoFine= tempoFineInput.value;
            var errorMessageRicarica3 = document.getElementById('errorMessageRicarica3');
            tempoFineInput.classList.remove('is-invalid');
            errorMessageRicarica3.style.display = 'none';

            if(!tempoFineInput){   
                errorMessageRicarica3.style.display = 'block';
                tempoFineInput.classList.add('is-invalid');
                return null;
            }

            const regexOra= new RegExp("^(?:[01]\\d|2[0-3]):[0-5]\\d$");
            if(!(regexOra.test(tempoFine))){
                errorMessageRicarica4.style.display = 'block'; 
                return null;
            }
    
        }

        // guardo se c'è numero telefonico
        if (!(phoneNumber.value))
            return "";       
        
        // controllo lunghezza numero
        if(!(phoneNumber.value.length === 10) && !(phoneNumber.value.length === 0)){ // se il numero inserito non è di 10 cifre e non è nullo
            phoneNumber.classList.add('is-invalid');
            errorMessagePhoneNumber.style.display= 'block';
            return null;
        }

        //controllo che non ci siano lettere o simboli
        var phoneNumberRegex= new RegExp("^\\d{10}$");
        if(!(phoneNumberRegex.test(phoneNumber.value))){
            phoneNumber.classList.add('is-invalid');
            errorMessagePhoneNumber2.style.display= 'block';
            return null;
        }

        
        return phoneNumber.value;
        
    }

}


