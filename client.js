const redis = require("redis");

const client = redis.createClient({
  host: process.env.REDIS_HOST,
  port: process.env.REDIS_PORT,
  password: process.env.REDIS_PASSWORD,
  user: process.env.REDIS_USER,
});
client.connect().then(() => {
  console.log("Connected to Redis");
  client.sendCommand(["keys", "*"]).then((keys) => {
    console.log(keys);
  });
});

client.disconnect();
