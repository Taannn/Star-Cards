// Create express app
import express from 'express'
import cors from 'cors'
import sqlite3 from 'sqlite3'
import bodyParser from 'body-parser'

var app = express()

const DBSOURCE = "ladder.db"

// Run the database SQLite
let db = new sqlite3.Database(DBSOURCE, (err) => {
    if (err) {
      // Cannot open database
      console.error(err.message)
      throw err
    }else{
        console.log('Connected to the SQLite database.')
    }
});

app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());
app.use(cors())

// Server port
var HTTP_PORT = 3000

// Start server
app.listen(HTTP_PORT, () => {
    console.log("Server running on port %PORT%".replace("%PORT%", HTTP_PORT))
});

// Root endpoint
app.get("/", (req, res, next) => {
    res.json({ "message": "Ok" })
});

//Get top 10 score
app.get("/api/ladder", (req, res, next) => {
    var sql = "select * from score order by number_years desc limit 10"
    var params = []
    db.all(sql, params, (err, rows) => {
        if (err) {
            res.status(400).json({ "error": err.message });
            return;
        }
        res.json({ "ladder": rows})
    });
});

//Insert a new score
app.post("/api/ladder/", (req, res, next) => {
    var errors = []
    if (errors.length) {
        res.status(400).json({ "error": errors.join(",") });
        return;
    }
    var data = {
        name: req.body.name,
        number_years: req.body.number_years
    }
    var sql = 'INSERT INTO score (name, number_years) VALUES (?,?)'
    var params = [data.name, data.number_years]
    db.run(sql, params, function(err, result) {
        if (err) {
            res.status(400).json({ "error": err.message })
            return;
        }
        res.json({ "message": "Ok" })
    });
})

// Default response for any other request
app.use(function(req, res) {
    res.status(404);
});