using Humanizer;

namespace TypicalTechTools.Services
{
    public class FileUploaderService
    {
        string _uploadPath = string.Empty;
        private readonly EncryptionService _encryptionService;
        public FileUploaderService(IWebHostEnvironment env, EncryptionService encryptionService)
        {
            _uploadPath = Path.Combine(env.WebRootPath, "Uploads");
            _encryptionService = encryptionService;
        }

        public void SaveFile(IFormFile file)
        {
            //Get the file name from the file
            string fileName = file.FileName;
            //Create a byte array to hold the file data
            byte[] fileContents;
            //Use a using statment to create a memory stream for processing the
            //file.This will allow us to convert the file into bytes from its orginal format
            using (var stream = new MemoryStream())
            {
                //Copy the file into the stream then pass it back out as an array to
                //the byte array
                file.CopyTo(stream);
                fileContents = stream.ToArray();
            }
            
            //Pass the file to the encryption service to be encrypted and store the result.
            var encyptedFile = _encryptionService.EncryptByteData(fileContents);

            //Pass the encrypted file Data into a new memory stream as part of a using statement.
            using (var dataStream = new MemoryStream(encyptedFile))
            {
                //Set the file's full name by appending it to the upload path
                var targetFile = Path.Combine(_uploadPath, fileName);
                using (var fileStream = new FileStream(targetFile,FileMode.Create))
                {
                    dataStream.WriteTo(fileStream);
                }
            }
        }

        public byte[] DownloadFile(String fileName)
        {
            //Call the method for reading the file that is named.
            var originalFile = ReadFileIntoMemory(fileName);
            //If the file is null or empty, return null.
            if (originalFile == null || originalFile.Length == 0)
            {
                return null;
            }

            //Pass the file data to the encryption service to be decrypted.
            var decryptedData = _encryptionService.DecryptByteData(originalFile);

            //Return the decrypted file to the caller.
            return decryptedData;
        }

        private byte[] ReadFileIntoMemory(string fileName)
        {
            //Request the named file from the Uploads folder
            var file = LoadFile(fileName);
            //If no file is found, return null.
            if (file == null)
            {
                return null;
            }
            //Create a  Memory stream to convert our file to byte data
            using (var stream = new MemoryStream())
            {
                //Use a file stream to open the file and read it
                using (var fileStream = File.OpenRead(file.FullName))
                {
                    //Copy the file into the memory stream to convert it to bytes
                    fileStream.CopyTo(stream);
                    //Return the bytes as an array to the caller.
                    return stream.ToArray();
                }
            }
        }

        private FileInfo LoadFile(string fileName)
        {
            //Create a directory object and get the directory(file) details of the uploads folder
            DirectoryInfo directory = new DirectoryInfo(_uploadPath);
            //Cycle through the files in the directory and find the one match the file name.
            var file = directory.EnumerateFiles()
                                .Where(f => f.Name.Equals(fileName))
                                .FirstOrDefault();
            //Return the file, or null if it is not found.
            
            return file;
        }
        

    }
}
