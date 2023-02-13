const redis = require("redis");

const client = redis.createClient({
  host: process.env.REDIS_HOST,
  port: process.env.REDIS_PORT,
});

client.auth(process.env.REDIS_PASSWORD);

client.on("connect", () => {
  console.log("Connected to Redis");
  
  client.sendCommand(["keys", "*"]).then((keys) => {
    console.log(keys);
  });
});

client.disconnect();
