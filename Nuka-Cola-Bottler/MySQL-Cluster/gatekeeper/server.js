'use strict';

const express = require('express');

const PORT = 8180;
const HOST = '0.0.0.0';

var tokens = {}

const app = express();
app.put('/:key/:value', (req, res) => {
  if (req.params.key && req.params.value) { 
	tokens[req.params.key] = req.params.value;	
	res.sendStatus(200);
  }
  else
    res.sendStatus(400);
});

app.get('/:key', (req, res)=> {
  if (req.params.key && tokens[req.params.key]) {
     res.send(tokens[req.params.key]);
  }
  else
     res.sendStatus(400);
});

app.listen(PORT, HOST);
console.log(`Listening on http://${HOST}:${PORT}`);
