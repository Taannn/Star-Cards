import fs from 'fs'
import sqlite3 from 'sqlite3'
const path = './ladder.db'

// If the database exists, drops the database
if (fs.existsSync(path)) {
    fs.unlinkSync(path)
}

var db = new sqlite3.Database(path);

// Create the table score with a name and the number_years
// number_years field is the power'year of the user
db.serialize(function() {
    db.run(`create table score (
                name varchar(100) not null,
                number_years integer not null
            );`)
});

db.close();