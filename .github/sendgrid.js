#! /usr/bin/env node

const sendGridMailClient = require("@sendgrid/mail");

sendGridMailClient.setApiKey(process.env.SENDGRID_API_KEY);

const message = {
  to: "liamgneville@gmail.com",
  from: "ustascraper@gmail.com",
  subject: "USTA Rankings Update",
  text: process.env.EMAIL_BODY,
  html: `<p>${process.env.EMAIL_BODY}</p>`,
};

sendGridMailClient
  .send(message)
  .then(() => console.log("Mail sent successfully"))
  .catch((error) => console.error(error.toString()));
