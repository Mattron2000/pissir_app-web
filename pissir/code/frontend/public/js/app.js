"use strict";   


class App{

    constructor(appContainer, navBtnToolBar){

        this.appContainer= appContainer;
        this.navBtnToolbar= navBtnToolBar;
        this.utenteLoggato= null;   //contiene id utente, username e grado
    
        this.homeListenersDefined= false;

        this.addLoginListener();

        /* routes Page.js */

        /* home page */
        page('/home', () =>{
            this.appContainer.innerHTML= '';    //appContainer è Element
            this.showHomePage();
        });

        /* area amministratore */
        page('/adminArea', () =>{
            this.appContainer.innerHTML= '';
            this.showAdminPage();
        });

        page('*', () => {
            page('/home');
        });

        /* inizializzazione, così fa binding di tutte le funzioni sopra*/
        page();
    }

    showHomePage(){
        
        this.appContainer.innerHTML= createHomePage();  //definita nel template
        //se nessuno è loggato tolgo pulsante per abbonarsi a premium
        if(this.utenteLoggato == null || this.utenteLoggato.type == "ADMIN")
            document.getElementById("premiumButton").style.display = 'none';    //nascondo bottone navbar per diventare premium
        this.prenotaBtnCheck();
        

        if (this.homeListenersDefined == true) 
            return; 


        // Codice per chiudere il menu su schermi piccoli quando si clicca su una voce
        document.addEventListener("DOMContentLoaded", function() {
            const navbarNavLinks = document.querySelectorAll(".navbar-nav .nav-link");
        
            navbarNavLinks.forEach(link => {
                link.addEventListener("click", function() {
                    const navbarCollapse = document.querySelector(".navbar-collapse");
                    const bsCollapse = new bootstrap.Collapse(navbarCollapse, {toggle: false});
                    bsCollapse.hide(); // Chiude il menu a tendina
                });
            });
        });

       
        this.homeListenersDefined = false;  
  

    }

    /* validazione email */
    isEmailValid(email) {
        const regex = new RegExp("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
        return regex.test(email);
      }

    /* gestione bottone login/logout */
    switchLogButton(){
        this.navBtnToolbar.innerHTML='';

        //se non c'è ancora stato login, mostro login
        if(this.utenteLoggato== null){
            this.navBtnToolbar.innerHTML= createLoginButton();
            return;
        }


        //se utente è loggato 

        this.navBtnToolbar.innerHTML= createLogOutButton(); //mostro logout

        //eventListener per fare logOut 
        $('#logout-btn').one('click', async (event) => {        
            event.preventDefault();

            this.utenteLoggato= null;
            document.getElementById("dettagliRicarica").innerHTML= "In questo box compariranno i dettagli relativi alle tue prenotazioni/multe!";

            this.switchLogButton(); //ri creo bottone login
            location.reload();

            page('/home');
        });
    }

    /* fa partire login */
    addLoginListener(){

        this.addSignupListener();   //aggiungo listener in caso l'utente voglia registrarsi
        const loginMod= document.querySelector('#login-form');
        //faccio partire login quando utente invia form
        document.getElementById('loginBtn').addEventListener('click', async(event) =>{
            event.preventDefault();

            let usernameElem= loginMod.elements['login-form-email'];
            let username= usernameElem.value;
            let passwordElem= loginMod.elements['login-form-password'];
            let password= passwordElem.value;

            let isMail= this.isEmailValid(username);
            if(!isMail){    //se mail non è nel formato corretto
                usernameElem.classList.add('is-invalid');
                if (passwordElem.classList.contains('is-invalid'))
                    passwordElem.classList.remove('is-invalid');
                document.getElementById('usernameError').innerText= "Errore: la mail inserita non è nel formato corretto.";
                return;
            }
            if(!password){
                if (usernameElem.classList.contains('is-invalid'))
                    usernameElem.classList.remove('is-invalid');
                passwordElem.classList.add('is-invalid');
                document.getElementById('passwordError').innerText= "Errore: inserire la password";
                return;
            }

            
            let response= await Api.loginUser(username, password);

            if(response.error){  //se ho inserito mail o password sbagliate
    
                if (usernameElem.classList.contains('is-invalid'))
                    usernameElem.classList.remove('is-invalid');
                if (passwordElem.classList.contains('is-invalid'))
                    passwordElem.classList.remove('is-invalid');
                document.getElementById('passwordError').innerText= "Errore: mail o password errati.";

                document.getElementById("errorModalBody").innerText= response.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }

            //se login è avvenuto correttamente
            this.utenteLoggato= response;   //mail e grado
            this.switchLogButton(); // trasformo bottone login in logout

            //se base o premium compare opzione per diventare utente premium
            if(this.utenteLoggato.type == "BASE" || this.utenteLoggato.type == "PREMIUM"){
                if(document.getElementById("premiumButton").style.display == 'none');    
                    document.getElementById("premiumButton").style.display = 'block';
            }

            if(this.utenteLoggato.type == "PREMIUM"){
                document.getElementById("abbonatiParagraph").textContent= "Sei già un utente premium, desideri annullare l'abbonamento e tornare ad essere un utente base?Ricorda: non potrai più prenotare preventivamente le tue soste";
                document.getElementById("abbonatiOBaseBtn").textContent= "Torna base";
            }
            if(this.utenteLoggato.type == "BASE"){
                document.getElementById("abbonatiParagraph").textContent= "Abbonati con il costo di 5 euro al mese per diventare un utente premium! In questo modo, potrai prenotare preventivamente le tue soste.";
                document.getElementById("abbonatiOBaseBtn").textContent= "Abbonati";
            }

            this.addSubscriptionListener();

            //riempi box con multe e prenotazioni, se ci sono
            if(this.utenteLoggato.type== "PREMIUM" || this.utenteLoggato.type == "ADMIN"){

                let myFines = await Api.getFines(this.utenteLoggato.email);
                if(myFines.error){  

                    document.getElementById("errorModalBody").innerText= myFines.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }
                
                if(myFines.length != 0){                   
                    let paragrafo= document.getElementById('dettagliRicarica');
                    let cont=0;
                    myFines.forEach(myFine => {
                        //metti nel box le multe
                        let orarioInizioMulta= myFine.datetime_start.replace("T", " ");
                        let orarioFineMulta= myFine.datetime_end.replace("T", " ");
                        //creo elemento html per la singola multa
                        let multaDiv = document.createElement("div");
                        multaDiv.style.textAlign = "center"; 
                        multaDiv.classList.add("premiumFine");
                        multaDiv.id = "multa" + cont;
                        multaDiv.innerHTML= `<br><br>Hai una multa da pagare!<br>Data di arrivo: ${orarioInizioMulta}<br>Data di fine: ${orarioFineMulta}<br>
                                            <button class="btn myButtonsMulta btn-sm d-block mx-auto" id="multaBtn${cont}">Paga</button>`;
                        paragrafo.append(multaDiv);
                        let btnId= `multaBtn${cont}`; 
                        this.addFinePaymentListener(this.utenteLoggato.email, btnId, multaDiv.id, myFine.datetime_start, myFine.datetime_end);
                        
                        cont= cont+1;
                    });
                        
                    paragrafo.scrollIntoView({ behavior: 'smooth' }); // Scroll verso l'elemento 
                }
                
                //cerco prenotazioni in db e metto in box
                let myPrenotations= await Api.getMyPrenotations(this.utenteLoggato.email);
                if(myPrenotations.error){  

                    document.getElementById("errorModalBody").innerText= myPrenotations.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }               
                if(myPrenotations.length != 0){
                    let paragrafo= document.getElementById('dettagliRicarica');
                    let cont=0;
                    myPrenotations.forEach(prenotation => {
                        //metto nel box le prenotazioni
                        let orarioInizio= prenotation.datetime_start.replace("T", " ");
                        let orarioFine= prenotation.datetime_end.replace("T", " ");
                        paragrafo.innerHTML+= `<br><br>Hai una prenotazione in sospeso!<br>Data di arrivo: ${orarioInizio}<br>Data di fine: ${orarioFine}<br>ID parcheggio assegnato: ${prenotation.slot_id}<br>
                                                <button class="btn myButtons btn-sm d-block mx-auto" id="cancellaPrenBtn${cont}">Cancella</button><br><br>`;
                        let cancellaPrenId= `cancellaPrenBtn${cont}`; 
                        this.addDeletePrenListener(this.utenteLoggato.email, cancellaPrenId, prenotation.datetime_start);
                        cont= cont+1;
                    });
                        
                    paragrafo.scrollIntoView({ behavior: 'smooth' }); // Scroll verso l'elemento 
                }
                
            }

            //chiudo modal
            $('#loginModal').modal('hide');
            loginMod.reset();   //tolgo input vecchi dai campi

            //se l'admin si è loggato mostro il bottone "Area amministratore"
            if(this.utenteLoggato.type == "ADMIN"){
                page('/adminArea');
            }
            
        });
    }

    /* listener cancella prenotazione */
    addDeletePrenListener(email, cancellaPrenBtn, datetime_start){

        let self= this;
        document.getElementById(cancellaPrenBtn).addEventListener('click', async() =>{

            let deleteData= {};
            deleteData.email= email;
            deleteData.inizio= datetime_start;
            let res= await Api.deletePrenotation(deleteData);
            if(res.error){
                document.getElementById("errorModalBody").innerText= res.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }

            $('#DelPrenModal').modal('show');

        });
    }

    /* listener pagamenti multe */
    addFinePaymentListener(email, btnId, multaDiv, orarioInizioMulta, orarioFineMulta){
        //a backend passo solo email e inizio
        let self= this;
        document.getElementById(btnId).addEventListener('click', async() =>{

            //ottengo prezzo orario 
            let prices= await Api.getPrices();
            if(prices.error){  

                document.getElementById("errorModalBody").innerText= prices.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }
            let hoursPrice= prices.costo_sosta;

            //calcolo tempo di sosta
            let dataFine= new Date(orarioFineMulta);
            let dataInizio= new Date(orarioInizioMulta);
            let tPermMillisec= dataFine.getTime() - dataInizio.getTime();
            let tPermMinutes = tPermMillisec / (1000 * 60);

            // calcolo il totale da pagare
            let prezzoTot = parseFloat((((parseFloat(hoursPrice) * parseInt(tPermMinutes)) / 60)).toFixed(2));
            
            //aprire modal per pagamento
            let finePaymentModal= new bootstrap.Modal(document.getElementById('finePaymentModal'));
            let finePaymentForm= document.querySelector('#finePaymentForm');
            let paragTotPagamento = document.querySelector('#paragTotPagamento');
            //aggiungo prezzo da pagare   
            paragTotPagamento.innerHTML = `Totale da pagare: ${prezzoTot}.`;            
            //paragTotPagamento.style.display = 'block';
            finePaymentModal.show();

            //ora serve listener per pulsante "paga"
            let lastPaga= document.querySelector('#finePaymentButton');
            lastPaga.addEventListener('click', async() =>{

                let credCardValRes= self.finePaymentValidation();
                if(credCardValRes == -1 || credCardValRes === undefined)
                    return;

                let dataF ={};
                dataF.email= email;
                dataF.orarioInizioMulta= orarioInizioMulta;
                let updateFineRes = await Api.updateFine(dataF);
                if(updateFineRes.error){
                    document.getElementById("errorModalBody").innerText = updateFineRes.error;
                    $('#errorModal').modal('show');                    
                    return;
                }

                document.getElementById("dettPrenP").innerText = "Pagamento avvenuto con successo.";
                $('#OkPrenModal').modal('show');
                finePaymentModal.hide();
                let remDiv = document.getElementById(multaDiv);
                remDiv.remove();



            });
            
        });
    }

    /* validazione dati pagamento multa */
    finePaymentValidation(){
        const finePaymentForm = document.querySelector('#finePaymentForm');

        const cardNumberInput = finePaymentForm.elements['cardNumberF'];
        const expirationDateInput = finePaymentForm.elements['expirationDateF'];
        const cvvInput = finePaymentForm.elements['cvvF'];
        const fineErrorMessageCreditCard1 = document.querySelector('#fineErrorMessageCreditCard1');
        const fineErrorMessageCreditCard2 = document.querySelector('#fineErrorMessageCreditCard2');
        const fineErrorMessageCreditCard3 = document.querySelector('#fineErrorMessageCreditCard3');
        const fineErrorMessageCreditCard4 = document.querySelector('#fineErrorMessageCreditCard4');
    
        //rimuovo classi errore precedenti
        cardNumberInput.classList.remove('is-invalid');
        expirationDateInput.classList.remove('is-invalid');
        cvvInput.classList.remove('is-invalid');
        fineErrorMessageCreditCard1.style.display= 'none';
        fineErrorMessageCreditCard2.style.display= 'none';
        fineErrorMessageCreditCard3.style.display= 'none';
        fineErrorMessageCreditCard4.style.display= 'none';

        // Verifico che sia inserito il numero carta
        if (!(cardNumberInput.value)) { 
            fineErrorMessageCreditCard1.style.display = 'block'; 
            cardNumberInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Verifico che sia inserita data scadenza
        if (!(expirationDateInput.value)) {  
            fineErrorMessageCreditCard2.style.display = 'block'; 
            expirationDateInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Verifico che sia inserito il cvv
        if (!(cvvInput.value)) {
            fineErrorMessageCreditCard3.style.display = 'block'; 
            cvvInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Regex per la validazione
        const cardNumberRegex = new RegExp("^\\d{4}(?: \\d{4}){3}$|^\\d{16}$"); // "^\\d{4}(?: \\d{4}){3}$" -> accetta il formato con spazi, "|" -> or, "d{16}" qualsiasi sequenza di 16 CIFRE da 0 a 9.
        const expirationDateRegex = new RegExp("^(0[1-9]|1[0-2])\\/\\d{2}$"); // Formato: MM/YY
        const cvvRegex = new RegExp("^\\d{3}$"); // CVV a 3 cifre
    
        if(cardNumberRegex.test(cardNumberInput.value) && expirationDateRegex.test(expirationDateInput.value) && cvvRegex.test(cvvInput.value)){
            fineErrorMessageCreditCard1.style.display = 'none';
            fineErrorMessageCreditCard2.style.display = 'none';
            fineErrorMessageCreditCard3.style.display = 'none';    
            fineErrorMessageCreditCard4.style.display = 'none';      
            return 0;
        } else {
            fineErrorMessageCreditCard4.style.display = 'block';
            let lastP= document.getElementById("dettPrenP");
            lastP.innerHTML= `I dati inseriti non sono corretti.`;
            return -1;
        }
    }


    /* listener per il bottone per diventare premium o tornare utente base */
    addSubscriptionListener(){

        $('#abbonatiOBaseBtn').one('click', async() =>{

            let newUserType= null;
            if(this.utenteLoggato.type == "BASE")
                newUserType= "PREMIUM";
            if(this.utenteLoggato.type == "PREMIUM")
                newUserType= "BASE";


            let subRes = await Api.setTypeUser(this.utenteLoggato.email, newUserType);

            if(subRes.error){
                document.getElementById("errorModalBody").innerText = subRes.error;
                $('#errorModal').modal('show');
                
                return;
            }
            //cambio il grado dell'utente
            this.utenteLoggato.type= newUserType;   
            
            //cambio modal tipo utente
            if(this.utenteLoggato.type == "PREMIUM"){
                document.getElementById("abbonatiParagraph").innerHTML= "Sei già un utente premium, desideri annullare l'abbonamento e tornare ad essere un utente base?<br>Ricorda: non potrai più prenotare preventivamente le tue soste";
                document.getElementById("abbonatiOBaseBtn").textContent= "Torna base";
            }
            if(this.utenteLoggato.type == "BASE"){
                document.getElementById("abbonatiParagraph").textContent= "Abbonati con il costo di 5 euro al mese per diventare un utente premium! In questo modo, potrai prenotare preventivamente le tue soste.";
                document.getElementById("abbonatiOBaseBtn").textContent= "Abbonati";
            }
            return;
        });

    }


    /* listener per gestione iscrizione/ creazione account */
    addSignupListener(){

        const signupForm = document.querySelector('#signup-form');

        $('#signupBtn').one('click', async(event) =>{
            event.preventDefault();

            let userEmail = signupForm.elements['signup-form-email'].value;
            let userPassword = signupForm.elements['signup-form-password'].value;    
            
            let response= await Api.insertNewUserCredentials(userEmail, userPassword); //nel DB credenziali

            if(response.error){  

                document.getElementById("errorModalBody").innerText= response.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }


            //se la registrazione è andata a buon fine, faccio login automatico
            let utenteLoggato1= await Api.loginUser(userEmail, userPassword);

            //inizializzo variabili per gestione accesso e sessione
            this.utenteLoggato= utenteLoggato1;

            // cambio login in logout
            this.switchLogButton();
             //se base o premium compare opzione per diventare utente premium
             if(this.utenteLoggato.type == "BASE" || this.utenteLoggato.type == "PREMIUM"){
                if(document.getElementById("premiumButton").style.display == 'none');    
                    document.getElementById("premiumButton").style.display = 'block';
            }

            if(this.utenteLoggato.type == "PREMIUM"){
                document.getElementById("abbonatiParagraph").textContent= "Sei già un utente premium, desideri annullare l'abbonamento e tornare ad essere un utente base?Ricorda: non potrai più prenotare preventivamente le tue soste";
                document.getElementById("abbonatiOBaseBtn").textContent= "Torna base";
            }
            if(this.utenteLoggato.type == "BASE"){
                document.getElementById("abbonatiParagraph").textContent= "Abbonati con il costo di 5 euro al mese per diventare un utente premium! In questo modo, potrai prenotare preventivamente le tue soste.";
                document.getElementById("abbonatiOBaseBtn").textContent= "Abbonati";
            }


            $('#signupModal').modal('hide');
            $('#loginModal').modal('hide');

            signupForm.reset();   //tolgo input vecchi dai campi

            page('/home');

        });
    } 

    prenotaBtnCheck(){
        let self=this;
        document.getElementById('prenota-btn').addEventListener("click", async function (event) {
            

            if(!(self.utenteLoggato) || self.utenteLoggato.type == "BASE"){
                let userNotPremiumModal = new bootstrap.Modal(document.getElementById('notPremiumModal'));
                userNotPremiumModal.show();
            }else{
                $('#modalPrenotazione').modal('show'); // Apre il modal
                self.addPrenotaListener();               
            }
            
            
        });
     
    }

    /*listener prenota*/
    addPrenotaListener(){

        let self=this;
        let myParking=null;
        
        document.getElementById('disponibilità-prenota-btn').addEventListener('click',  async function ottieniPrenotazione() {
            let validationRes= self.prenotaFormValidation();

            if(validationRes == -1){
                return;
            }

            let prenotaForm= document.getElementById("prenota-form");
            let tempoRic = prenotaForm.elements['tempoPermanenza'].value;
            let arrivingTime= prenotaForm.elements['tempoArrivo'].value;
            let dataPrenotaz= prenotaForm.elements['dataPren'].value;

            arrivingTime= arrivingTime + ":00";

            let arrivingTimeDate= new Date(`${dataPrenotaz}T${arrivingTime}`);  //Thu Mar 27 2025 17:00:00 GMT+0100 (Ora standard dell’Europa centrale)
            let tempoRicInt = parseInt(tempoRic, 10); // Base 10

            let arrivingTimeMillisec= arrivingTimeDate.getTime();
            let tempoRicMillisec= (tempoRicInt * 60) * 1000;
            let finalTimeDate = new Date(arrivingTimeMillisec + tempoRicMillisec);


            /* mi servono stringa con solo data e stringa con solo ora */

            //solo data
            let annoFinePrenotazione = finalTimeDate.getFullYear();
            let meseFinePrenotazione = String(finalTimeDate.getMonth() + 1).padStart(2, '0'); // Mesi da 0 a 11, quindi +1
            let giornoFinePrenotazione = String(finalTimeDate.getDate()).padStart(2, '0');           
            // Formatto la data in "yyyy-mm-dd"
            let formattedDate = `${annoFinePrenotazione}-${meseFinePrenotazione}-${giornoFinePrenotazione}`;
            //solo ora
            // Estraggo i componenti dell'ora
            let hours = String(finalTimeDate.getHours()).padStart(2, '0');
            let minutes = String(finalTimeDate.getMinutes()).padStart(2, '0');
            let seconds = String(finalTimeDate.getSeconds()).padStart(2, '0');

            // Formatto l'ora in "hh:mm:ss"
            let formattedTime = `${hours}:${minutes}:${seconds}`;
            
            //devo restituire idParcheggio               
            //myParking sarà un json
            let dataInizioCompleta = `${dataPrenotaz}T${arrivingTime}`;
            let dataFineCompleta = `${formattedDate}T${formattedTime}`;
            let datiPren= {};
            datiPren.arrivingTime=dataInizioCompleta;
            datiPren.endTime=dataFineCompleta;

            // date per visualizzazione a schermo SENZA "T"
            let dataInizioCompletaFront = `${dataPrenotaz} ${arrivingTime}`;
            let dataFineCompletaFront = `${formattedDate} ${formattedTime}`;
            

            myParking= await Api.getMyCarParkingSosta(datiPren); 

            /* se tutti i parcheggi della fascia oraria non sono disponibili */
            //let paragrafo = document.getElementById('dettagliRicarica');
            if(myParking.error == "No free slots"){ 
                // Mostra il modal
                let slotNotAvailableModal = new bootstrap.Modal(document.getElementById('slotNotAvailableModal'));
                slotNotAvailableModal.show();
                /*paragrafo.innerHTML= `Errore: impossibile prenotare all'orario desiderato`;
                paragrafo.scrollIntoView({ behavior: 'smooth' }); // Scroll verso l'elemento
                */
            }
            else{

                /* chiedo conferma e di inserire i dati della carta */

                // Mostro il modal
                $('#creditCardModal').modal('show'); // Apre il modal
                let paragrafoIDParcheggio = document.getElementById('paragIDParcheggio');
                paragrafoIDParcheggio.innerHTML= `Potrai parcheggiare al posto numero ${myParking}.<br>Inserisci i dati della tua carta di credito per confermare.`
                $('#submitCardButton').one('click', async function(event) {
                    event.preventDefault();
                    let credCardValRes= self.creditCardValidation();
                    if(credCardValRes == -1 || credCardValRes === undefined)
                        return;
                    

                    /* aggiungo dettagli servizio al db */
                    let newParkingData= {};
                    newParkingData.email= self.utenteLoggato.email;
                    newParkingData.slot_id= myParking;
                    newParkingData.datetime_start= dataInizioCompleta;
                    newParkingData.datetime_end = dataFineCompleta;
                    newParkingData.percentage= null;
                    newParkingData.reservation= true;
                    
                    let regResponse= await Api.insertDetails(newParkingData);
                    $('#creditCardModal').modal('hide'); 

                    if(regResponse.error){  

                        document.getElementById("errorModalBody").innerText= regResponse.error;
                        $('#errorModal').modal('show');
                        return; //si blocca la richiesta.
                    }

                    let lastP= document.getElementById("dettPrenP");
                    lastP.innerHTML= `Prenotazione confermata!<br>Data di arrivo: ${dataInizioCompletaFront}<br>Data di fine: ${dataFineCompletaFront}<br>ID parcheggio assegnato: ${myParking}<br>Potrai trovare tutti i dettagli relativi alle tue prenotazioni e ad eventuali multe nel box "Le mie prenotazioni/multe".`;
                    let paragrafo= document.getElementById('dettagliRicarica');
                    paragrafo.innerHTML += `<br>Prenotazione confermata!<br>Data di arrivo: ${dataInizioCompletaFront}<br>Data di fine: ${dataFineCompletaFront}<br>ID parcheggio assegnato: ${myParking}<br>`;
                    paragrafo.scrollIntoView({ behavior: 'smooth' }); // Scroll verso l'elemento  
                
                });    

                //senza questa ultima riga, se provo a ricaricare più volte di fila su parcheggi diversi, mantiene memorizzato e manda una query anche per tutti i tentativi precedenti
                document.getElementById("disponibilità-prenota-btn").removeEventListener('click', ottieniPrenotazione); 
            }   
        });

            
                
    }

    prenotaFormValidation(){

        var tArrivoInput = document.getElementById('tempoArrivo');
        var tPermanenzaInput = document.getElementById('tempoPermanenza');
        var dataPren= document.getElementById('dataPren');
        var errorMessagePrenota = document.getElementById('errorMessagePrenota');
        var errorMessagePrenota1 = document.getElementById('errorMessagePrenota1');
        var errorMessagePrenota2 = document.getElementById('errorMessagePrenota2');
        var errorMessagePrenota3 = document.getElementById('errorMessagePrenota3');
        var errorMessagePrenota4 = document.getElementById('errorMessagePrenota4');
        var errorMessagePrenota15 = document.getElementById('errorMessagePrenota15');
        
        var tArrivoValue = tArrivoInput.value;
        var tempoPermValue= tPermanenzaInput.value;
        var dataPrenValue= dataPren.value;
        

        // Rimuovo le classi di errore precedenti
        tArrivoInput.classList.remove('is-invalid');
        tPermanenzaInput.classList.remove('is-invalid');
        dataPren.classList.remove('is-invalid');
        errorMessagePrenota.style.display = 'none';
        errorMessagePrenota1.style.display = 'none';
        errorMessagePrenota2.style.display = 'none';
        errorMessagePrenota3.style.display = 'none';
        errorMessagePrenota4.style.display = 'none'; 
        errorMessagePrenota15.style.display = 'none';    

        // Verifico che sia inserita l'ora di arrivo
        if (!tArrivoValue) {      
            errorMessagePrenota.style.display = 'block'; 
            tArrivoInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Verifico che sia inserita la data
        if (!dataPrenValue) {      
            errorMessagePrenota2.style.display = 'block'; 
            dataPren.classList.add('is-invalid');  
            return -1;         
        } 

        // Verifico che sia inserito il tempo di permanenza
        if (!tempoPermValue) {      
            errorMessagePrenota1.style.display = 'block'; 
            tPermanenzaInput.classList.add('is-invalid'); 
            return -1;          
        } 

        /* verifico che i dati inseriti siano nel formato valido (regex) */

        const regexData= new RegExp("^\\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\\d|3[01])$");
        const regexOra= new RegExp("^(?:[01]\\d|2[0-3]):[0-5]\\d$");

        if(!(regexData.test(dataPrenValue)) || !(regexOra.test(tArrivoValue)) || !(Number.isInteger(parseInt(tempoPermValue)))){

            errorMessagePrenota15.style.display = 'block'; 
            return -1;
        }

        /*verifico che la data inserita non sia precedente a quella odierna*/

        // Ottengo la data odierna
        const today = new Date();
        today.setHours(0, 0, 0, 0); // Imposto l'ora a mezzanotte per confronto solo con la data
        // Converto la stringa nel formato yyyy-mm-gg in un oggetto Date
        const [year, month, day] = dataPrenValue.split("-").map(Number);
        const inputDate = new Date(year, month - 1, day); // I mesi in JavaScript vanno da 0 (gennaio) a 11 (dicembre)
        // Confronto la data
        if(inputDate < today){ // true se la data è precedente a oggi
            errorMessagePrenota3.style.display = 'block'; 
            dataPren.classList.add('is-invalid'); 
            return -1;
        }

        /*verifico che se si prenota per la data odierna, l'ora arrivo sia successiva all'ora attuale*/
        
        if(!(inputDate < today) && !(inputDate > today)){
    
            let now3 = new Date();
            let currentHours = now3.getHours();
            let currentMinutes = now3.getMinutes();
            
            // Estraggo ore e minuti da tArrivoInput
            let [inputHours, inputMinutes] = tArrivoValue.split(":").map(Number);

            // Controllo se l'orario inserito è successivo all'orario attuale
            if (!(inputHours > currentHours || (inputHours === currentHours && inputMinutes > currentMinutes))) {
                errorMessagePrenota4.style.display = 'block'; 
                tArrivoInput.classList.add('is-invalid');   
                return -1;
            }
        }

        $('#modalPrenotazione').modal('hide');  //fa chiudere il modal se i dati sono corretti
        return 0;
        
    }

    creditCardValidation(){
        const cardNumberInput = document.getElementById('cardNumber');
        const expirationDateInput = document.getElementById('expirationDate');
        const cvvInput = document.getElementById('cvv');
        const errorMessageCreditCard1 = document.getElementById('errorMessageCreditCard1');
        const errorMessageCreditCard2 = document.getElementById('errorMessageCreditCard2');
        const errorMessageCreditCard3 = document.getElementById('errorMessageCreditCard3');
        const errorMessageCreditCard4 = document.getElementById('errorMessageCreditCard4');
    
        //rimuovo classi errore precedenti
        cardNumberInput.classList.remove('is-invalid');
        expirationDateInput.classList.remove('is-invalid');
        cvvInput.classList.remove('is-invalid');
        errorMessageCreditCard1.style.display= 'none';
        errorMessageCreditCard2.style.display= 'none';
        errorMessageCreditCard3.style.display= 'none';
        errorMessageCreditCard4.style.display= 'none';

        // Verifico che sia inserito il numero carta
        if (!(cardNumberInput.value)) { 
            errorMessageCreditCard1.style.display = 'block'; 
            cardNumberInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Verifico che sia inserita data scadenza
        if (!(expirationDateInput.value)) {  
            errorMessageCreditCard2.style.display = 'block'; 
            expirationDateInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Verifico che sia inserito il cvv
        if (!(cvvInput.value)) {
            errorMessageCreditCard3.style.display = 'block'; 
            cvvInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Regex per la validazione
        const cardNumberRegex = new RegExp("^\\d{4}(?: \\d{4}){3}$|^\\d{16}$"); // "^\\d{4}(?: \\d{4}){3}$" -> accetta il formato con spazi, "|" -> or, "d{16}" qualsiasi sequenza di 16 CIFRE da 0 a 9.
        const expirationDateRegex = new RegExp("^(0[1-9]|1[0-2])\\/\\d{2}$"); // Formato: MM/YY
        const cvvRegex = new RegExp("^\\d{3}$"); // CVV a 3 cifre
    
        if(cardNumberRegex.test(cardNumberInput.value) && expirationDateRegex.test(expirationDateInput.value) && cvvRegex.test(cvvInput.value)){
            errorMessageCreditCard1.style.display = 'none';
            errorMessageCreditCard2.style.display = 'none';
            errorMessageCreditCard3.style.display = 'none'; 
            errorMessageCreditCard4.style.display = 'none';           
            return 0;
        } else {
            errorMessageCreditCard4.style.display = 'block';
            let lastP= document.getElementById("dettPrenP");
            lastP.innerHTML= `I dati inseriti non sono corretti.`;
            return -1;
        }
    }
    

    /* parte per admin */

    async showAdminPage(){

        this.appContainer.innerHTML = createAdminPage();
        this.addAdminGetParkingsListener(); //monitoraggio parcheggi
        this.addAdminUpdateHoursPrice();    //aggiornare costo $/ora
        this.addAdminUpdateKwPrice();   //aggiornare costo per kw
        this.addAdminGetPaymentsHistory() //ricerca cronologia pagamenti
    }

    /*monitoraggio parcheggi*/
    addAdminGetParkingsListener(){

        let self=this;
        $('#monitoraggio-parcheggi-btn').one('click', async() =>{

            let getParkingsMonitoringDef= await Api.getMonitoring();

            if(getParkingsMonitoringDef.error){  

                document.getElementById("errorModalBody").innerText= getParkingsMonitoringDef.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }

            //MOSTRARE OUTPUT
            self.generateOutputTable(getParkingsMonitoringDef);

        });

    }

    /*aggiornare costo $/ora*/
    addAdminUpdateHoursPrice(){

        let self=this;
        document.getElementById('updateHoursBtn').addEventListener('click', async() =>{

            let admForm1 = document.querySelector('#updateHoursForm');
            let getPrice= admForm1.elements["updateHoursInput"].value;

            //validazione
            if(!getPrice){
                admForm1.elements['updateHoursInput'].classList.add('is-invalid');
                document.getElementById('updateHoursError1').innerText= "Inserire il nuovo prezzo orario";
                return;
            }


            if(!((Number.isFinite(parseFloat(getPrice))) && getPrice>0)){
                admForm1.elements['updateHoursInput'].classList.add('is-invalid');
                document.getElementById('updateHoursError2').innerText= "Errore: il nuovo prezzo deve essere un numero positivo";
                return;
            }


            let newHoursPrice= await Api.updateHoursPrice(parseFloat(getPrice));
            if(newHoursPrice.error){  

                document.getElementById("errorModalBody").innerText= newHoursPrice.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }

            //MOSTRARE OUTPUT
            self.generateOutputTable(newHoursPrice);

            $('#updateHoursModal').modal('hide');
            admForm1.reset();
        });
        
    }

    /*aggiornare costo al kw*/
    addAdminUpdateKwPrice(){

        let self=this;
        document.getElementById('updateKwBtn').addEventListener('click', async() =>{

            let admForm2 = document.querySelector('#updateKwForm');
            let getPrice= admForm2.elements["updateKwInput"].value;

            //validazione
            if(!getPrice){
                admForm2.elements['updateKwInput'].classList.add('is-invalid');
                document.getElementById('updateKwError1').innerText= "Inserire il nuovo prezzo per kw";
                return;
            }


            if(!((Number.isFinite(parseFloat(getPrice))) && getPrice>=1)){
                admForm2.elements['updateKwInput'].classList.add('is-invalid');
                document.getElementById('updateKwError2').innerText= "Errore: il nuovo prezzo deve essere un numero positivo";
                return;
            }


            let newKwPrice= await Api.updateKwPrice(parseFloat(getPrice));
            if(newKwPrice.error){  

                document.getElementById("errorModalBody").innerText= newKwPrice.error;
                $('#errorModal').modal('show');
                return; //si blocca la richiesta.
            }

            self.generateOutputTable(newKwPrice);

            $('#updateKwModal').modal('hide');
            admForm2.reset();
        });
        
    }

    /*ricerca cronologia pagamenti*/
    
    addAdminGetPaymentsHistory(){

        let self=this;
        document.getElementById('paymentsHistoryBtn').addEventListener('click', async() =>{

            let paymentsHistory= await self.paymentsHistoryOptions();
            $('#paymentHistoryModal').modal('hide');

            //MOSTRARE OUTPUT
            self.generateOutputTable(paymentsHistory);
        });

    }

    async paymentsHistoryOptions(){

        var oraInizioInputAdm = document.getElementById('startTime');
        var oraFineInputAdm = document.getElementById('endTime');
        var dataStartInputAdm= document.getElementById('startDate');
        var dataEndInputAdm= document.getElementById('endDate');
        
        var oraInizioAdmValue = oraInizioInputAdm.value;
        var oraFineAdmValue = oraFineInputAdm.value;
        var dataStartAdmValue= dataStartInputAdm.value;
        var dataEndAdmValue= dataEndInputAdm.value;


        //checkbox 1
        let onlyParking = document.getElementById("onlyParking");
        let onlyCharging = document.getElementById("onlyCharging");
        //checkbox 2
        let basicUsers = document.getElementById("basicUsers");
        let premiumUsers = document.getElementById("premiumUsers");

        let timeHistory=null;   // per query date/ore
        let typeHistory= null;  // per risultati/query su tipo servizio
        let gradeUserHistory = null;    // per risultati/query su grado utente
        let defHistory= null;   //elenco finale
        

        //se è inserito almeno uno di questi dati faccio query, per le altre opzioni farò un semplice filtraggio su questa query.
        if((oraInizioAdmValue && oraFineAdmValue) || (dataStartAdmValue && dataEndAdmValue)){

            //gestione solo DATA
            if(!oraInizioAdmValue && !oraFineAdmValue){  //c'è solo data

                let datesHistory={};
                datesHistory.dataInizio=dataStartAdmValue;
                datesHistory.dataFine=dataEndAdmValue;
                timeHistory= await Api.getAdmHistoryByDate(datesHistory);
                if(timeHistory.error){  

                    document.getElementById("errorModalBody").innerText= timeHistory.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }
    
            }
            //gestione solo fascia oraria
            if(!dataStartAdmValue && !dataEndAdmValue){
                let hoursHistory={};
                hoursHistory.oraInizio= oraInizioAdmValue;
                hoursHistory.oraFine= oraFineAdmValue;
                timeHistory= await Api.getAdmHistoryByHours(hoursHistory);
                if(timeHistory.error){  

                    document.getElementById("errorModalBody").innerText= timeHistory.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }
               
            }
            
            // data e fascia oraria
            if(oraInizioAdmValue && oraFineAdmValue && dataStartAdmValue && dataEndAdmValue){
                let datesHistory={};
                datesHistory.dataInizio=dataStartAdmValue;
                datesHistory.dataFine=dataEndAdmValue;
                let onlyDateHistory= await Api.getAdmHistoryByDate(datesHistory);

                if(onlyDateHistory.error){  

                    document.getElementById("errorModalBody").innerText= timeHistory.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }
                
                let hoursHistory={};
                hoursHistory.oraInizio= oraInizioAdmValue;
                hoursHistory.oraFine= oraFineAdmValue;
                let onlyHoursHistory= await Api.getAdmHistoryByHours(hoursHistory);
                if(onlyHoursHistory.error){  

                    document.getElementById("errorModalBody").innerText= onlyHoursHistory.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }

                const idSet = new Set(onlyDateHistory.map(item => item.email));
                // Filtro il secondo array per tenere solo gli elementi con IDerogazione in comune
                timeHistory = onlyHoursHistory.filter(item => idSet.has(item.email));
               
            }

            //ora se anche checkbox, FILTRO il risultato

            //checkbox soste/ricariche, filtro i risultati

            //cerco solo soste
            if(onlyParking.checked && !onlyCharging.checked){
                typeHistory = timeHistory.filter(item => item.kw == null);
            }
            //cerco solo ricariche
            if(onlyCharging.checked && !onlyParking.checked){
                typeHistory = timeHistory.filter(item => item.kw > 0);
            }
            //se non sono checkati/ sono checkati tutti e due, non cambia niente

            let tempHistory=null;
            if(typeHistory == null)
                tempHistory = timeHistory;
            else
                tempHistory= typeHistory;


            // ora controllo checkbox per tipologia utente

            //se cerco solo base
            if(basicUsers.checked && !premiumUsers.checked){
                gradeUserHistory= tempHistory.filter(item => item.type == "BASE");
            }
            //se cerco solo premium
            if(premiumUsers.checked && !basicUsers.checked){
                gradeUserHistory= tempHistory.filter(item => item.type == "PREMIUM");
            }

            //se non sono checkati/ sono checkati tutti e due, non cambia niente
            
            if(gradeUserHistory == null)
                defHistory= tempHistory;
            else
                defHistory= gradeUserHistory;


            //torno risultato. solo se ho filtri data/ora
            return defHistory;

            //ora in defHistory ho tutti i possibili filtraggi
        }else{  //SE NON HO FILTRI DATA O ORA query se no prendo dal db qualsiasi riga. troppo!
            //se no query al db
            //se sono tutti checkati o nessuno, prendo tutto il db. NON HO FILTRI DI NESSUN TIPO
            if((onlyParking.checked && onlyCharging.checked && basicUsers.checked && premiumUsers.checked) || (!onlyParking.checked && !onlyCharging.checked && !basicUsers.checked && !premiumUsers.checked)){
                let allHistory= await Api.getAdmAllHistory();
                if(allHistory.error){  

                    document.getElementById("errorModalBody").innerText= allHistory.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }
                
                return allHistory;
            }

            //primo checkbox (tipologia servizio)
            //cerco solo soste
            if(onlyParking.checked && !onlyCharging.checked){
                typeHistory = await Api.getAdmTypeHistory("PARKING");
                
                if(typeHistory.error){  

                    document.getElementById("errorModalBody").innerText= typeHistory.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }
            }
            //cerco solo ricariche
            if(onlyCharging.checked && !onlyParking.checked){
                typeHistory = await Api.getAdmTypeHistory("CHARGING");
             
                if(typeHistory.error){  

                    document.getElementById("errorModalBody").innerText= typeHistory.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }
            } 

            //secondo checkbox (grado utente)
            //se non è stato cercato ancora niente, faccio query
            if(typeHistory == null){
                //se cerco solo base
                if(basicUsers.checked && !premiumUsers.checked){
                    gradeUserHistory= await Api.getAdmHistoryByGrade("BASE");
                
                    if(gradeUserHistory.error){  

                        document.getElementById("errorModalBody").innerText= gradeUserHistory.error;
                        $('#errorModal').modal('show');
                        return; //si blocca la richiesta.
                    }
                }
                //se cerco solo premium
                if(premiumUsers.checked && !basicUsers.checked){
                    gradeUserHistory= await Api.getAdmHistoryByGrade("PREMIUM");
         
                    if(gradeUserHistory.error){  

                        document.getElementById("errorModalBody").innerText= gradeUserHistory.error;
                        $('#errorModal').modal('show');
                        return; //si blocca la richiesta.
                    }
                }

            }else{  // ho i risultati solo per tipo pagamento, filtro! (no query)
                //se cerco solo base
                if(basicUsers.checked && !premiumUsers.checked){
                    gradeUserHistory= typeHistory.filter(item => item.type == "BASE");
                  
                }
                //se cerco solo premium
                if(premiumUsers.checked && !basicUsers.checked){
                    gradeUserHistory= typeHistory.filter(item => item.type == "PREMIUM");
   
                }
            }
            if(gradeUserHistory == null)
                return typeHistory;
            return gradeUserHistory;

        }
        
    }


    generateOutputTable(outputData){

        let table= document.getElementById("admTable");
        table.replaceChildren(); 

        /* header tabella */
        let tHeadElem= document.createElement('thead');
        table.appendChild(tHeadElem);

        let trElem= document.createElement('tr');
        tHeadElem.appendChild(trElem);

        
        /* contenuto tabella */
        let tBodyElem= document.createElement('tbody');
        table.appendChild(tBodyElem);

        /* se ho solo un oggetto */
        if(outputData[0]=== undefined){
            if(outputData.length==0){
                //se ho lista vuota
                tBodyElem.classList.add("myText");
                tBodyElem.innerHTML= "Nessun risultato";
                return;
            }

            //header
            for(let resElem in outputData){
                let thElem= document.createElement('th');
                thElem.scope= "col";
                thElem.innerHTML= resElem;
                trElem.appendChild(thElem);
            }

            //body
            let trElem1= document.createElement('tr');
            tBodyElem.appendChild(trElem1);
    
            //giro su singolo oggetto dati
            for(let resElem in outputData){
                let tdElem= document.createElement('td');
                tdElem.innerHTML= outputData[resElem];
                trElem1.appendChild(tdElem);
            }

        }
        /* se ho un array come risultato */
        else{
            //giro solo su primo elemento per prendere i nomi delle proprietà
            let firstRow= outputData[0];
            //giro su singolo oggetto
            for(let resElem in firstRow){
                let thElem= document.createElement('th');
                thElem.scope= "col";
                thElem.innerHTML= resElem;
                trElem.appendChild(thElem);
            }

            outputData.forEach(outputObj => {

                let trElem= document.createElement('tr');
                tBodyElem.appendChild(trElem);

                //giro su singolo oggetto
                for(let resElem in outputObj){
                    let tdElem= document.createElement('td');
                    tdElem.innerHTML= outputObj[resElem];
                    trElem.appendChild(tdElem);
                }
            }); 
        }        

    }
}
