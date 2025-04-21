"use strict";

const express = require('express') ;
const app = express() ;

const morgan = require('morgan');   //info richieste http(logging)


app.use(morgan('tiny'));
app.use(express.json());    //middleware: ogni cosa passa da qui prima di raggiungere la funzione da utilizzare. Viene convertito il corpo in json
app.use(express.static(__dirname + '/public'));

app.get('/', (req, res) => {
    res.redirect('index.html');
});

app.get('/entranceTotem/', (req, res) => {
    res.redirect('indexEntranceTotem.html');
});

app.get('/exitTotem/', (req, res) => {
    res.redirect('indexExitTotem.html');
});

app.get('*', (req, res) => {
    res.redirect('index.html');
});

const PORT = 3000;

app.listen(PORT, () =>
    console.log(`Server in ascolto sulla porta ${PORT}`)
); 