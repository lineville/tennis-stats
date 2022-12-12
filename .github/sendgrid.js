#! /usr/bin/env node

const sgMail = require('@sendgrid/mail');
sgMail.setApiKey(process.env.SENDGRID_API_KEY);

const msg = {
    to: 'liamgneville@gmail.com',
    from: 'ustascraper@gmail.com',
    subject: 'Updated USTA Rankings',
    text: 'New rankings are available!',
    html: '<p>New rankings are available!</p>',
};

sgMail
    .send(msg)
    .then(() => console.log('Mail sent successfully'))
    .catch(error => console.error(error.toString()));