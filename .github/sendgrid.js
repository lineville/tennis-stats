#! /usr/bin/env node

const sgMail = require('@sendgrid/mail');
sgMail.setApiKey(process.env.SENDGRID_API_KEY);

const msg = {
    to: 'liamgneville@gmail.com',
    from: 'ustascraper@gmail.com',
    subject: 'USTA Rankings Update',
    text: process.env.EMAIL_BODY,
    html: `<p>${process.env.EMAIL_BODY}</p>`,
};

sgMail
    .send(msg)
    .then(() => console.log('Mail sent successfully'))
    .catch(error => console.error(error.toString()));