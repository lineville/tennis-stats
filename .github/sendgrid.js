#! /usr/bin/env node

const sendGridMailClient = require("@sendgrid/mail");
sendGridMailClient.setApiKey(process.env.SENDGRID_API_KEY);

const trimmedEmailBody = (str) => {
  while (!str.startsWith("<h2>") && str !== "") {
    str = str.split("\n").slice(1).join("\n");
  }
  return str;
};

const message = {
  to: process.env.TO_EMAIL,
  from: process.env.FROM_EMAIL,
  subject: "USTA Rankings Update",
  text: trimmedEmailBody(process.env.EMAIL_BODY),
  html: trimmedEmailBody(process.env.EMAIL_BODY),
};

sendGridMailClient
  .send(message)
  .then(() => console.log("Mail sent successfully"))
  .catch((error) => console.error(error.toString()));
