#! /usr/bin/env node

const sendGridMailClient = require("@sendgrid/mail");
sendGridMailClient.setApiKey(process.env.SENDGRID_API_KEY);

const showdown = require("showdown");
const converter = new showdown.Converter();
const html = converter.makeHtml(process.env.EMAIL_BODY);

const message = {
  to: process.env.TO_EMAIL,
  from: process.env.FROM_EMAIL,
  subject: "USTA Rankings Update",
  text: process.env.EMAIL_BODY,
  html: html,
};

sendGridMailClient
  .send(message)
  .then(() => console.log("Mail sent successfully"))
  .catch((error) => console.error(error.toString()));
