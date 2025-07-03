import React from "react";
import "./ToggleButton.css";

interface Props {
  isExploded: boolean;
  onToggle: () => void;
}

export const ToggleButton: React.FC<Props> = ({ isExploded, onToggle }) => (
  <button className="ToggleButton" onClick={onToggle}>
    {isExploded ? "Show 3D View" : "Show Exploded View"}
  </button>
);
