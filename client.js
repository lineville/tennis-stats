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

const keys = await client.sendCommand(["keys","*"])

console.log(keys)

client.disconnect();
