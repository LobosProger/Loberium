# Loberium
**Loberium Payment System**

Loberium Payment System is a blockchain-based project that utilizes the Unity game engine to simulate a payment system inspired by Bitcoin principles. To run this project, a minimum of 3 computers connected to a local network (via Ethernet or Wi-Fi) with the Unity engine installed is required.

### How to Use:

1. Launch the project on one computer and click on the "Start Host" button in the top-left corner of the screen. An internet icon will appear.

2. On other computers with Unity and the project running, click the "Find" button also located in the top-left corner and wait. Click on the first IP address that appears, followed by the computer icon in the center of the screen.

3. Clicking on the computer icon designates the computer as a network user, allowing it to send "coins" over the network. Clicking on the server icon designates the computer as a miner responsible for processing network payments, earning a reward of 50 coins for each mined block.

4. After connecting, the current coin balance and address of the user will be displayed at the top of the screen. A field for inputting the recipient's address, the amount of coins to send, and a send button are available at the bottom.

5. Miners will process transactions in real-time, dynamically displaying the processing progress on the screen. If a user lacks sufficient coins, miners will display a message indicating insufficient funds for the transaction.

6. In case of enough coins, miners will display a message about including the transaction in the blockchain block and will mine a hash that meets the difficulty criteria, dynamically showing the selected hash on the screen.

7. Upon successful processing, the sender's and recipient's coin balances will be updated on the screen, and the first miner to process the transaction will display a message about receiving a reward for their efforts, akin to the Bitcoin system.

### Unique Concept:

The uniqueness of the idea lies in the absence of a dedicated server for processing payments and transactions. Everything occurs in a distributed manner, similar to the Bitcoin payment system.
