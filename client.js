const redis = require("redis");

const client = redis.createClient({
  password: process.env.REDIS_PASSWORD,
  socket: {
    host: process.env.REDIS_HOST,
    port: process.env.REDIS_PORT,
  },
});

client.on("error", function(err) {
  console.log("Error " + err);
});

// Sets the key "octocat" to a value of "Mona the octocat"
client.set("octocat", "Mona the Octocat", redis.print);

client.keys("species", function (err, replies) {
  console.log(replies.length + " replies:");
  replies.forEach(function (reply, i) {
      console.log("    " + i + ": " + reply);
  });
  client.quit();
});
// client.on("error", (error) => {
//   console.error(error);
// });

// client.on("connect", () => {
//   console.log("Connected to Redis");

//   client.sendCommand(["keys", "*"]).then((keys) => {
//     console.log(keys);
//   });
// });
