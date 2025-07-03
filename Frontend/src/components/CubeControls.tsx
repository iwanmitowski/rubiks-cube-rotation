import React from 'react';
import { Face } from '../types/api';
import './CubeControls.css';

interface CubeControlsProps {
  onMove: (face: Face, clockwise: boolean) => void;
  onReset: () => void;
  isDisabled?: boolean;
}

const MOVES: { name: string; face: Face }[] = [
  { name: 'U', face: Face.Up },
  { name: 'D', face: Face.Down },
  { name: 'L', face: Face.Left },
  { name: 'R', face: Face.Right },
  { name: 'F', face: Face.Front },
  { name: 'B', face: Face.Back },
];

export const CubeControls: React.FC<CubeControlsProps> = ({ onMove, onReset, isDisabled = false }) => {
  return (
    <div className="CubeControls-container">
      {MOVES.map(({ name, face }) => (
        <div key={name} className="CubeControls-group">
          <button
            title={`${name} (Clockwise)`}
            className="CubeControls-button"
            onClick={() => onMove(face, true)}
            disabled={isDisabled}
          >
            {name}
          </button>
          <button
            title={`${name}' (Counter-Clockwise)`}
            className="CubeControls-button"
            onClick={() => onMove(face, false)}
            disabled={isDisabled}
          >
            {name}'
          </button>
        </div>
      ))}
      <button
        className="CubeControls-button CubeControls-button--reset"
        onClick={onReset}
        disabled={isDisabled}
      >
        Reset Cube
      </button>
    </div>
  );
};