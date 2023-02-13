const redis = require("redis");

const client = redis.createClient({
  password: process.env.REDIS_PASSWORD,
  socket: {
    host: process.env.REDIS_HOST,
    port: process.env.REDIS_PORT,
  },
});

client.on("connect", () => {
  console.log("Connected to Redis");

  client.sendCommand(["keys", "*"]).then((keys) => {
    console.log(keys);
  });
});

client.disconnect();
