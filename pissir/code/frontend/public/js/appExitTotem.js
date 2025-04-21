"use strict";   

class AppExitTotem{

    constructor(exitTotemappContainer){

        this.appContainer= exitTotemappContainer;
        this.utenteLoggato= null;   //contiene id email, type

        this.homeListenersDefined= false;

        /* routes Page.js */

        /* login page */
        page('/exitLogin', () =>{
            this.appContainer.innerHTML= '';    //appContainer è Element
            this.showEntranceLoginPage();
        });

        /* pagamento page */
        page('/paymentPage', () =>{
            this.appContainer.innerHTML= '';    //appContainer è Element
            this.showPaymentPage();
        });


        page('*', () => {
            page('/exitLogin');
        });

        /* inizializzazione, così fa binding di tutte le funzioni sopra*/
        page();
    }


    showEntranceLoginPage(){
        
        this.appContainer.innerHTML= createEntranceTotemLoginPage();  //definita nel template
        this.addExitTotemLoginListener();
        if (this.homeListenersDefined == true) 
            return; 

        this.homeListenersDefined = false;   //se no tornando indietro non va più il bottone, si "disattiva" il listener
    }

    /* fa partire login */
    addExitTotemLoginListener(){

        //faccio partire login quando utente invia form
        $('#loginBtnEntranceTotem').one('click', async(event) =>{
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

                return; //si blocca la richiesta.
            }

            //se login è avvenuto correttamente, carico pagina sosta/prenota
            this.utenteLoggato= response;   //email, type
            page('/paymentPage');

        });
    }

    checkService(myService) {
        // Ottengo la data e l'ora correnti
        let now = new Date();
        let fine= new Date(myService.datetime_end);

        // Calcolo la differenza in millisecondi
        let diffInMilliseconds = Math.abs(now.getTime() - fine.getTime());
        // Converto la differenza in minuti
        let diffInMinutes = diffInMilliseconds / (1000 * 60);

        // Controllo se la differenza è entro 15 minuti
        if(diffInMinutes <= 15)
            return myService;
        return null;
    }


    async showPaymentPage(){

        //ottengo i prezzi 
        let prices= await Api.getPrices();
        if(prices.error){  

            document.getElementById("errorModalBody").innerText= prices.error;
            $('#errorModal').modal('show');
            return; //si blocca la richiesta.
        }
        let kwPrice= prices.costo_kw + "€/kw";
        let hoursPrice= prices.costo_sosta + "€/h";

        //cerca in db dove "pagato=0" e data fine coerente con data e ora attuali
        let myServicesList= await Api.getUnpaidUserRequest(this.utenteLoggato.email);
        

        if(myServicesList.error){  

            document.getElementById("errorModalBody").innerText= myServicesList.error;
            $('#errorModal').modal('show');
            return; //si blocca la richiesta.
        }

        if(myServicesList.length == 0){ //ERRORE
            let errorServiceModal = new bootstrap.Modal(document.getElementById('errorServiceModal'));
            errorServiceModal.show();
            this.utenteLoggato= null;
            page('/exitLogin');           
            return;
        }
        
        let myService= null;
        for (let service of myServicesList) {
            myService= this.checkService(service);  //torna servizio se coincide con orario attuale, altrimenti null
            if(myService)
                break;
        }

        if(!myService){ //se non è registrato un servizio coerente con l'orario attuale
            let errorServiceModal = new bootstrap.Modal(document.getElementById('errorServiceModal'));
            errorServiceModal.show();
            this.utenteLoggato= null;
            page('/exitLogin');
            return;
        }
        //N.B. tutti gli utenti rispettano la durata dichiarata (fino a 15 minuti prima o 15 minuti dopo)

        //visualizzo
        this.appContainer.innerHTML= createExitTotemPaymentPage();  //definita nel template
        //ora ho dettagli servizio. Li mostro.

        //creo orari nel formato per visualizzazione, senza "T"
        let orarioInizio= myService.datetime_start.replace("T", " ");
        let orarioFine= myService.datetime_end.replace("T", " ");
        let prezzoTot;
        //se kw == null, prezzoTot= (hoursPrice * TempoPermanenza)/60
  
        if(myService.kw == null){
            let dataFine= new Date(myService.datetime_end);
            let dataInizio= new Date(myService.datetime_start);
            let tPermMillisec= dataFine.getTime() - dataInizio.getTime();
            let tPermMinutes = tPermMillisec / (1000 * 60);
            prezzoTot = parseFloat((((parseFloat(hoursPrice) * parseInt(tPermMinutes)) / 60)).toFixed(2));
            document.getElementById("totemServiceDetails").innerHTML = `
                ID parcheggio assegnato: ${myService.slot_id}<br>
                Data di arrivo: ${orarioInizio}<br>
                Data di fine: ${orarioFine}<br>
                Durata sosta: ${tPermMinutes} minuti<br>
            `;
        }
        else{   // se è ricarica,  // kwPrice : 1kw = prezzoTot : PercentualeRic   =>  prezzoTot= kwPrice * PercentualeRic
            prezzoTot = parseFloat(((parseFloat(kwPrice)) * (parseInt(myService.kw))).toFixed(2));
            document.getElementById("totemServiceDetails").innerHTML = `
                ID parcheggio assegnato: ${myService.slot_id}<br>
                Data di arrivo: ${orarioInizio}<br>
                Data di fine: ${orarioFine}<br>
                Kw erogati: ${myService.kw}<br>
            `;
        }
        
        //devo visualizzare i dettagli e pulsante paga.

        document.getElementById("ricPrice").textContent += kwPrice;
        document.getElementById("hPrice").textContent += hoursPrice;
                        
        // Rendo visibile il paragrafo
        document.getElementById("totemServiceDetails").classList.remove("d-none");  
        // visualizzo tot pagamento
        document.getElementById("priceToPay").textContent += ` ${prezzoTot}€`;

        //utente schiaccia "paga"
        this.addPaymentListener(myService.datetime_end);

    }

    //listener pulsante per aprire modal pagamento
    addPaymentListener(oraFine){

        let self=this;
        $('#paga-btn').one('click', async () =>{

            let paymentModal = new bootstrap.Modal(document.getElementById('creditCardModal'));
            paymentModal.show();

            $('#submitCardButton').one('click', async function(event) {
                event.preventDefault();
                let credCardValRes= self.creditCardValidation();

                //se dati inseriti non sono corretti, mostro errore e li si corregge
                if(credCardValRes == -1 || credCardValRes === undefined)
                    return;
                //se dati giusti, pago: modifico la riga del db e setto "Pagato" a 1.
                let oggi5 = new Date(); // Ottengo la data attuale
                let [ore, minuti, secondi] = oraFine.split(":").map(Number);
                oggi5.setHours(ore, minuti, secondi, 0);
          
                let resPayment= await Api.updatePayment(self.utenteLoggato.email);
                if(resPayment.error){  

                    document.getElementById("errorModalBody").innerText= resPayment.error;
                    $('#errorModal').modal('show');
                    return; //si blocca la richiesta.
                }

               // resettare form se no rimangono i dati della carta dell'utente prima di me 
                self.resetPaymentForm();
                paymentModal.hide();

                let lastP= document.getElementById("avvisoPaymentParagraph");
                lastP.innerHTML= `Pagamento effettuato con successo. Arrivederci!`;

                //logout e torno su pagina principale
                setTimeout(() => {
                    this.utenteLoggato= null;
                    $('#OkPaymentModal').modal('hide');
                    page('/exitLogin');
                }, 8000);
            });
            
        });
    }

    //controllo dati inseriti nel modal per pagare
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

        // Verifica che sia inserito il numero carta
        if (!(cardNumberInput.value)) { 
            errorMessageCreditCard1.style.display = 'block'; 
            cardNumberInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Verifica che sia inserita data scadenza
        if (!(expirationDateInput.value)) {  
            errorMessageCreditCard2.style.display = 'block'; 
            expirationDateInput.classList.add('is-invalid');   
            return -1;        
        } 

        // Verifica che sia inserito il cvv
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
            let lastP= document.getElementById("avvisoPaymentParagraph");
            lastP.innerHTML= `I dati inseriti non sono corretti.`;
            return -1;
        }
    }

    resetPaymentForm(){
        document.getElementById('cardNumber').value = "";
        document.getElementById('expirationDate').value= "";
        document.getElementById('cvv').value= "";
    }
}