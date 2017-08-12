var mongo = require('mongodb');
var gremlin = require('gremlin-secure');

var seen = [];
var count = 0;

var assert = require('assert');
var MongoClient = mongo.MongoClient;
var GremlinClient = gremlin.createClient(443, 'peoplegraph.graphs.azure.com', {"session": false, "ssl":true, "user": "/dbs/graphdb/colls/people", "password": "..."});
var userName = process.argv.length == 3 ? process.argv.slice(2)[0] : 'ND11';

console.log("Add relationships for user " + userName);

MongoClient.connect('...', function (err, db){
    assert.equal(null, err);
    console.log("Connected correctly to MongoDB API endpoint");
    var collection = db.collection('scotch-users');
    findUser(collection, userName, function(user){
        GremlinClient.execute("g.V().drop()",{}, (err, results) =>{
            if (err) return console.error(err);
            console.log("Connected correctly to Gremlin API endpoint");
            GremlinClient.execute("g.addV('people').property('id','" + user['username'] + "')",{}, (err, results) =>{
                if (err) return console.error(err);
                listUsers(collection, 20, function(users){
                    users.forEach(function (target){                    
                        writeGraphEdge(user, target);
                    });
                    db.close();
		   
		            var _flagCheck = setInterval(function() {
		                if (count >=20)
				        process.exit();}, 100);
                });
            });
        });
    });
});

var findUser = function (collection, name, callback) {
    collection.find({username: name}).limit(1).toArray(function(err, docs){
            assert.equal(null, err);
            assert.equal(1, docs.length);
            callback(docs[0]);
    });
}
var listUsers = function (collection, limit, callback) {    
    collection.find({}).limit(limit).toArray(function(err, docs){
            assert.equal(null, err);
            callback(docs);
    });
}

var writeGraphEdge = function (from, to){
    if (seen.indexOf(to['username'])<0 && from['username'] != to['username']){
        console.log('Adding edge: ' + from['username'] + ' --- knows ---> ' + to['username']);
        GremlinClient.execute("g.addV('people').property('id','" + to['username'] + "')",{}, (err, results) =>{
            if (err) return console.error(err);
            GremlinClient.execute("g.V('" + from['username'] +"').addE('knows').to(g.V('" + to['username'] + "'))", {}, (err, results)=>{
              if (err) return console.error(err);
	      count++;
            })
        });
        seen.push(to['username']);
    }
    else
	count++;
}