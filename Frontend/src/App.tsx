import { useState } from "react";
import { CubeViewer } from "./components/CubeViewer";
import { CubeControls } from "./components/CubeControls";
import { useCube } from "./hooks/useCube";
import "./App.css"; // Import the new CSS file

function App() {
  const {
    exploded,
    isLoadingView,
    isErrorView,
    resetCube,
    isResetting,
    moveCube,
    isMoving,
  } = useCube();

  const [isExplodedChecked, setIsExplodedChecked] = useState(false);
  const isActionPending = isMoving || isResetting;

  if (isLoadingView) {
    return (
      <div className="App-viewerContainer">
        <h1>Loading Cube...</h1>
      </div>
    );
  }

  if (isErrorView) {
    return (
      <div className="App-viewerContainer">
        <h1>Error: Could not load cube data.</h1>
      </div>
    );
  }

  if (!exploded) {
    return (
      <div className="App-viewerContainer">
        <h1>No cube data available.</h1>
      </div>
    );
  }

  return (
    <div className="App">
      <div className="App-viewerContainer">
        <CubeViewer view={exploded} exploded={isExplodedChecked} />
      </div>

      <div className="App-controlsContainer">
        <h2 className="App-title">Controls</h2>
        <CubeControls
          onMove={(face, clockwise) => moveCube({ face, clockwise })}
          onReset={() => resetCube()}
          isDisabled={isActionPending}
        />
        <hr className="App-divider" />
        <label className="App-checkboxLabel">
          <input
            type="checkbox"
            className="App-checkbox"
            checked={isExplodedChecked}
            onChange={(e) => setIsExplodedChecked(e.target.checked)}
          />
          Exploded View
        </label>
      </div>
    </div>
  );
}

export default App;
