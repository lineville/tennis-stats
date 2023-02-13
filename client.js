const redis = require("redis");

const client = redis.createClient({
  host: process.env.REDIS_HOST,
  port: process.env.REDIS_PORT,
  password: process.env.REDIS_PASSWORD,
  user: process.env.REDIS_USER,
});

client.on("error", function (err) {
  console.log("Error " + err);
});

console.log("Redis client connected");

client.quit();

// // Sets the key "octocat" to a value of "Mona the octocat"
// redisClient.set("octocat", "Mona the Octocat", redis.print);
// // Sets a key to "octocat", field to "species", and "value" to "Cat and Octopus"
// redisClient.hset("species", "octocat", "Cat and Octopus", redis.print);
// // Sets a key to "octocat", field to "species", and "value" to "Dinosaur and Octopus"
// redisClient.hset("species", "dinotocat", "Dinosaur and Octopus", redis.print);
// // Sets a key to "octocat", field to "species", and "value" to "Cat and Robot"
// redisClient.hset(["species", "robotocat", "Cat and Robot"], redis.print);
// // Gets all fields in "species" key

// redisClient.hkeys("species", function (err, replies) {
//   console.log(replies.length + " replies:");
//   replies.forEach(function (reply, i) {
//     console.log("    " + i + ": " + reply);
//   });
//   redisClient.quit();
// });
