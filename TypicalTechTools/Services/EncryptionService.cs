using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Security.Cryptography;


namespace TypicalTechTools.Services
{
    public class EncryptionService
    {
        string _secretKey;

        public EncryptionService(IConfiguration config)
        {
            _secretKey = config["SecretKey"];
        }

        public byte[] EncryptByteData(byte[] fileData)
        {
            //Create a using statment that generates an instance of the AES
            //encryption algorithm to use.
            //This will automatically generate an Initialization Vector (IV) which acts as an
            //additional random element to use for the encryption.
            using (var aesAlg = Aes.Create())
            {
                //Convert the Secret key to bytes and pass it to the algorithm
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(_secretKey);
                //Create an encryptor object using the algorithm and pass it the key and
                //Initialization Vector (IV) for it to use for the encryption stages.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key,aesAlg.IV);
                //Create a Memory stream to handle our final data and put it in an array
                //once it is encrypted
                using (var memStream = new MemoryStream())
                {
                    //Write the Initialization Vector to the start of the stream
                    memStream.Write(aesAlg.IV);
                    //Create a CryptoStream to use our algorithm and encrypt the data.
                    using (var cryStream = new CryptoStream (memStream, encryptor, CryptoStreamMode.Write))
                    {
                        //Pass the file to the crypto stream to be processed
                        cryStream.Write(fileData);
                        //Finalise the encryption and clear the stream
                        cryStream.FlushFinalBlock();
                        //Return the result from the memory stream as a byte[]
                        return memStream.ToArray();
                    }
                }
            }
        }

        public byte[] DecryptByteData(byte[] encryptedData)
        {
            //Create a using statement that generates an instance of the AES encryption 
            //algorithm to use.
            using (var aesAlg = Aes.Create())
            {
                //Convert the Secret key to bytes and pass it to the algorithm
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(_secretKey);
                //Create an array to hold the initialization vertor which we will be 
                //retrieving from the file
                byte[] IV = new byte[16];
                //Copy the first 16 bytes from the file data, this is where our IV was hidden
                //in the file.
                Array.Copy(encryptedData, IV, IV.Length);
                //Create a decryptor using the key and IV that will perform our decryption.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, IV);
                //Create a Memory stream to handle our final data and put it in an array
                //once it is decrypted
                using (var memStream = new MemoryStream())
                {
                    //Create a CryptoStream to use our algorithm and decrypt the data.
                    using (var cryStream = new CryptoStream(memStream,decryptor,CryptoStreamMode.Write))
                    {
                        //Pass the file to the crypto stream to be processed
                        cryStream.Write(encryptedData, IV.Length, encryptedData.Length - IV.Length);
                        //Finalise the decryption and clear the stream
                        cryStream.FlushFinalBlock();
                        //Return the result from the memory stream as a byte[]
                        return memStream.ToArray();
                    }
                }
            }
        }
    }
}
