## Importing Project into Unity

1. Download the ZIP from GitHub (Code → Download ZIP)
2. Extract the contents into any folder on your machine
3. Open Unity Hub
    - Go to Projects → Add
    - Select the folder you just extracted
4. The project requires Unity Editor version: `6000.1.10f1`  
   Make sure you have this version installed via Unity Hub

---
## Build and Run with WebGL.rar file
1. Extract WebGL.rar contents into a folder
2. Skip section below and follow steps to run locally.
## Build and Run (WebGL)

1. Open the project in Unity
2. From the top menu, go to File → Build Settings
3. Select WebGL as the build target
    - If not installed, click Add Modules from Unity Hub to install WebGL support
4. Click Switch Platform (if not already WebGL)
5. Click Build and Run
    - Choose a folder for the build (e.g., `Builds/WebGL`)
    - Unity will compile and launch it in your default browser

---

## Running WebGL Build Locally

Most browsers block WebGL from running correctly using `file://` URLs.  
To run the build locally without errors, you need to host it on a local server.

### Option 1: Using Node.js (`http-server`)

1. Make sure [Node.js](https://nodejs.org/) is installed
2. Run this once to install the server globally:
   ```bash
   npm install -g http-server
   cd path/to/your/WebGLBuild
   http-server
   Open in browser: http://localhost:8080

### Option 2: Using python
        cd path/to/your/WebGLBuild
        python -m http.server
        
