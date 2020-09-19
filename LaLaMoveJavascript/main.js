const API_KEY = "02bd00b1a3304b8a9175cbc4b7cc7f5d";
const SECRET = "MC0CAQACBQC+5UT/AgMBAAECBQCvL77RAgMAyAMCAwD0VQIDAK5dAgIsOQIC";
var CryptoJS = require("crypto-js");
const axios = require('axios');
const curlirize  = require( 'axios-curlirize');
const baseUrl="https://sandbox-rest.lalamove.com";
let json = require('./body.json');
const time = new Date().getTime().toString();
// console.log("time: "+time);
const method = 'POST';
// const path = '/v2/quotations';
const path = '/v2/orders';
const body = JSON.stringify(json);
// console.log(body);
const rawSignature = `${time}\r\n${method}\r\n${path}\r\n\r\n${body}`;

const SIGNATURE = CryptoJS.HmacSHA256(rawSignature, SECRET).toString();
const TOKEN = `${API_KEY}:${time}:${SIGNATURE}`;
// console.log(rawSignature);
// console.log("TOKEN: "+TOKEN);


curlirize(axios);
var url=`${baseUrl}${path}`;
async function makeRequest() {  
  const res = await axios.post(url, body, {
    headers: {
      'Authorization': `hmac ${TOKEN}`,
      'X-LLM-Country': 'VN',
      'X-Request-ID': 'e05383df-d42f-4bb0-b5f3-e56cfe7f717c'
    }
  });

  // console.log(res);
}

makeRequest();