# Tic Tac Toe - Full Stack Application

## How to Run Locally

### 1. Launch the .NET Backend
*   Open your terminal and navigate to the backend directory:
    ```bash
    cd backend/TicTacToe.Api
    ```
*   Start the API server:
    ```bash
    dotnet run
    ```
*   Note the local port assigned by the .NET server in the terminal output (e.g., `http://localhost:5000`).

### 2. Launch the Angular Frontend
*   Open a new terminal session and navigate to the frontend directory:
    ```bash
    cd frontend
    ```
*   Install the required project dependencies:
    ```bash
    npm install
    ```
*   Update the API connection URL: Open `frontend/src/app/services/game.ts` and modify line 8 to ensure the backend URL perfectly matches the port your .NET server is running on.
*   Start the Angular development server:
    ```bash
    npm start