export class RsaService {
  constructor() {
  }

  encrypt(publicKey : string, password : string)
  {
    var NodeRSA = require('node-rsa');
    var key = new NodeRSA({b: 2048});
    
    var text = 'Hello RSA!';
    var encrypted = key.encrypt(text, 'base64');
    console.log('encrypted: ', encrypted);
    var decrypted = key.decrypt(encrypted, 'utf8');
    console.log('decrypted: ', decrypted);
  }
}