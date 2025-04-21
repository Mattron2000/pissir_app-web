"use strict";

const backendPort = 9999;
const backendURL = "http://localhost:" + backendPort + "/api/v1/";

class Api{

    /*aggiunge utente al db */
    static insertNewUserCredentials=async (username, password) =>{

        let response= await fetch(backendURL + "register", {
            method: 'post',
            headers:{
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "email": username,
                "password": password
            })
        });

        return await response.json();
    };

    /*uso x fare login*/
    /*cerca utente nel db (CredenzialiUtente) per autenticazione*/
    static loginUser= async (username, password) =>{
        let url= backendURL + "login?email=" + username + "&password=" + password;

        let response= await fetch(url, {
            method: 'get',
            headers:{
                'Content-Type': 'application/json'
            }
        });

        return await response.json();
    }

    /* cambia tipo di utente */
    static setTypeUser= async(email, type) =>{

        let response= await fetch(backendURL + "users/" + email + "/type",{
            method: 'put',
            headers:{
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "type": type
            })
        });

        return await response.json();
    }

   /* richiesta info quando un utente Prenota */
    static getMyCarParkingSosta= async (dati) =>{
        let url = backendURL + "slots/firstfreeslot?datetime_start=" + dati.arrivingTime + "&datetime_end=" + dati.endTime;

        let response= await fetch(url, {
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /* ricerca prenotazione al login premium */
    static getMyPrenotations= async (email) =>{
        let url = backendURL + "reservations/" + email;

        let response= await fetch(url,{
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /* ricerca multe al login */
    static getFines= async (email) =>{
        let url = backendURL + "fines/" + email;

        let response= await fetch(url,{
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /* richiesta numero auto in coda */
    static getCarQueue= async () =>{
        let url = backendURL + "queue/size";

        let response= await fetch(url,{
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }


    /*aggiungere dettagli servizio al db*/
    static insertDetails=async (details) =>{

        let response= await fetch(backendURL + "reservations", {
            method: 'post',
            headers:{
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(details)

        });

        return await response.json();
    }

    /* pagare multe */
    /*static updateFine=async (email, orarioInizioMulta) =>{

        let response= await fetch(backendURL + "fines", {
            method: 'put',
            headers:{
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "email": email,
                "datetime_start": orarioInizioMulta
            })
        });

        return await response.json();
    }*/

    static updateFine=async (data) =>{

        let response= await fetch(backendURL + "fines", {
            method: 'put',
            headers:{
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "email": data.email,
                "datetime_start": data.orarioInizioMulta
            })
        });

        return await response.json();
    }


    /* ADMIN */

    /*richiesta monitoraggio parcheggi*/
    static getMonitoring=async () =>{

        let response= await fetch(backendURL + "slots",{
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /*aggiornare costo orario*/
    static updateHoursPrice=async (price) =>{

        let response= await fetch(backendURL + "admin/prices/hours", {
            method: 'put',
            headers:{
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(price)
        });

        return await response.json();
    }

    /*aggiornare costo per kw*/
    static updateKwPrice=async (price) =>{

        let response= await fetch(backendURL + "admin/prices/kw", {
            method: 'put',
            headers:{
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(price)
        });

        return await response.json();
    }

    /* ricerca cronologia per data */
    static getAdmHistoryByDate= async (dates) =>{
        // TODO fix "dataInizio" and "dataFine" Objects to String
        let url = backendURL + "admin/history/date?date_start=" + dates.dataInizio + "&date_end=" + dates.dataFine;

        let response= await fetch(url, {
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /* ricerca pagamenti per fascia oraria */
    static getAdmHistoryByHours= async (hours) =>{
        // TODO fix "oraInizio" and "oraFine" Objects to String
        let url = backendURL + "admin/history/time?time_start=" + hours.oraInizio + "&time_end=" + hours.oraFine;

        let response= await fetch(url, {
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /* ricerca tutti i pagamenti */
    static getAdmAllHistory= async () =>{

        let response= await fetch(backendURL + "admin/history",{
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /* ricerca pagamenti tipo servizio */
    static getAdmTypeHistory= async (tipo) =>{
        let url = backendURL + "admin/history/type/" + tipo;

        let response = await fetch(url, {
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /* ricerca pagamenti grado utente */
    static getAdmHistoryByGrade= async (grado) =>{
        let url = backendURL + "admin/history/grade/" + grado;

        let response = await fetch(url, {
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }


    /* API TOTEM */

    /* ottenere costo orario e al kw */
    static getPrices= async () =>{

        let response= await fetch(backendURL + "prices",{
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

    /* modificare prenotazione da sosta a ricarica */
    //qui cancella da tab Prenotazione, poi chiama insertDetails()
    static deletePrenotation= async(data) =>{

        let response= await fetch(backendURL + "reservations", {
            method: 'delete',
            headers:{
                'Accept': 'application/json'
            },
            body: JSON.stringify({
                "email": data.email,
                "datetime_start" : data.inizio
            })
        });

        return await response.json();
    }

    /* settare pagamento effettuato */
    static updatePayment=async (email) =>{

        let response= await fetch(backendURL + "payments/" + email, {
            method: 'put',
            headers:{
                'Content-Type' : 'application/json'
            }
        });

        return await response.json();
    }

    /* ottenere servizio appena effettuato da pagare al totem uscita */
    /* ricerca prenotazione al login premium */
    static getUnpaidUserRequest= async (email) =>{
        let url = backendURL + "requests/" + email + "/unpaid";

        let response= await fetch(url,{
            method: 'get',
            headers:{
                'Accept': 'application/json'
            }
        });

        return await response.json();
    }

}
