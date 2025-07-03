# Rubik's Cube Simulator

<p align="center">
  A 3D Rubik's Cube simulator built with React, Three.js, and .NET.
  <br>
  Rotate faces, view the 2D unfolded layout, and explore the mathematics of the cube.
</p>

---

## 📝 Table of Contents

- [About The Project](#about-the-project)
- [Features](#features)
- [Screenshots](#screenshots)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Local Development](#local-development)
  - [Production env – WIP](#production-env---wip)
- [Database Schema](#database-schema)
- [Testing](#testing)
- [Roadmap](#roadmap)

## 🧐 About The Project <a name="about-the-project"></a>

This project is a fully interactive, 3D simulation of a Rubik's Cube, developed as a Single Page Application (SPA). It demonstrates the integration of a modern frontend stack (React, Vite, Three.js) with a robust backend API built on .NET 8.

The core functionality allows any cube face to be rotated clockwise and counter-clockwise, with the 3D model updating in real-time. It also includes an "exploded" 2D view to display the cube's state as a flat matrix.

## ✨ Features <a name="features"></a>

- ✅ **Interactive 3D Cube:** A fully rotatable 3D model built with Three.js.
- ✅ **Intuitive Face Rotations:** Click buttons to rotate any of the 6 faces clockwise or counter-clockwise.
- ✅ **Unfolded 2D View:** Visualize the cube's state as a flat 9x12 matrix, making it easy to see all faces at once.
- ✅ **Backend API:** A .NET 8 API handles the cube's state logic and transformations.
- ✅ **Comprehensive Unit Tests:** The backend logic is thoroughly tested with xUnit.

## 📸 Screenshots <a name="screenshots"></a>

- Base State
  ![Imgur](https://i.imgur.com/NiiNgQW.png)

- Rotated Face State
  ![Imgur](https://i.imgur.com/2QbdoCJ.png)

- Exploded View
  ![Imgur](https://i.imgur.com/moRMqEl.png)

## 🛠️ Technology Stack <a name="technology-stack"></a>

### Frontend

- **[ReactJS](https://react.dev/):** A JavaScript library for building user interfaces.
- **[Vite](https://vitejs.dev/):** A modern, fast frontend build tool.
- **[Three.js](https://threejs.org/):** For rendering the 3D cube and handling animations.
- **[TypeScript](https://www.typescriptlang.org/):** For static typing and improved developer experience.
- **[Axios](https://axios-http.com/):** For making HTTP requests to the backend API.

### Backend

- **[.NET 8 API](https://dotnet.microsoft.com/):** The backend framework for serving cube logic.
- **[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore):** For generating Swagger/OpenAPI documentation.

### Development & Tools

- **[OpenAI](https://openai.com/):** Used to assist with complex mathematical calculations for cube rotations.
- **[xUnit](https://xunit.net/):** The testing framework for the .NET backend.
- **[Docker](https://www.docker.com/):** For containerizing the application (work in progress).

## 🏁 Getting Started <a name="getting-started"></a>

Follow these instructions to get a copy of the project up and running on your local machine.

### Prerequisites

You must have the following software installed:

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
- **[Node.js](https://nodejs.org/)** (which includes npm)
- **[Git](https://git-scm.com/)**
- **[Docker](https://www.docker.com/)**

### Local Development

1. **Clone the repository**
   ```sh
   git clone https://github.com/iwanmitowski/rubiks-cube-rotation.git
   cd Frontend
   npm install
   cd ..
   cd Backend
   cd RubiksCubeRotation
   ```
2. Open the .sln run the project

### Production env - WIP

1. **Clone the repository**
   ```sh
    git clone https://github.com/iwanmitowski/rubiks-cube-rotation.git
    cd Backend
    cd RubiksCubeRotation
    docker compose up -d
   ```

### 📈 Database Schema

The application is designed to eventually support user accounts and saved cube states. The planned database schema consists of two main tables:

1. **AspNetUsers**: The default table from ASP.NET Core Identity for user management.
2. **SavedCubes**: A custom table to store the state of a user's cube.

Here is the entity definition for a saved cube:

Generated csharp

```cs
class SavedCube
{
public int Id { get; set; }
public int UserId { get; set; }
public virtual ApplicationUser User { get; set; }
public string State { get; set; }
public DateTime LastModified { get; set; }
}
```

## 🧪 Testing <a name="testing"></a>

The backend logic for cube rotations and state management is covered by a suite of 24 xUnit tests to ensure correctness and prevent regressions.

- ![Imgur](https://i.imgur.com/LUvhn7J.png)
