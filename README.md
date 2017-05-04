# PasswordManager
Password Management utility created for final project of CS 460 Security Laboratory, a class at UIUC

Created by Brad Anderson and Ben Collins

# Usage
Open bin/PasswordManager.exe. Enter a username and password and click "Create User" to create a new password management account. Once logged in, login details can be stored by entering a password and a key to store it under in the Add/Update section and clicking "Add/Update". Passwords can then be retrieved by entering the associated key in the Get/Delete section and clicking "Get", or deleted by clicking "Delete". To change your password, click "File/Settings" and then follow the instructions.

# Technical Details
All usernames and passwords are stored on the user's hard drive inside of an encrypted SQLite database. When a user logs in, the database associated with their account is decrypted into a temporary working copy. When the user logs out or closes the application, this working copy is reencrypted with the user's password and saved back over the original file. When a user creates an account, their password is hashed and salted, and this hash is then stored to the disk alongside the user's account database. When a user logs in, their password is then hashed using the same salt and verified against the original. To encrypt and decrypt the database, the user's password is used to decrypt the hash generated from the original password, and this key is then used as the key and IV for AES encryption and decryption of the database. Because the user's password is required for encryption/decryption of the database, and this password is never stored on disk, the database is secure against decryption without the user entering their password. Further, the database is secure against dictionary and rainbow table attacks through the use of salted hashes.
